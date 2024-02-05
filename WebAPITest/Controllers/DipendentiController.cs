using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using WebAPITest.Classi;
using MySql.Data.MySqlClient;
using MySqlConnector;

namespace WebAPITest.Controllers
{
    [Route("Dipendenti")]
    [ApiController]

    //1A CHIAMATA: GET restituisce la lista di articoli 
    //2A CHIAMATA: GET restituisce un dipendente tramite DipendenteID
    //3A CHIAMATA: POST inserisce dipendente 
    //4A CHIAMATA: DELETE elimina dipendente 
    //5A CHIAMATA: PUT aggiorna l'email del dipendente
    //6A CHIAMATA: GET restituisce il CodiceFiscale del Dipendente inviato tramite DipendenteID. Se il DipendenteID non esiste lo comunica

    public class DipendentiController : ControllerBase
    {

        //1A CHIAMATA
        #region LISTA DIPENDENTI
        [Route("Lista Dipendenti")]
        [HttpGet]

        public List<Dipendente> GetListDipendenti()
        {
            //dichiaro variabile per connettermi al DB
            SqlConnection? mySQLConnection = null;

            //dichiaro e creo l'istanza
            List<Dipendente> myListDipendenti = new List<Dipendente>();

            try
            {

                String StringConnessione = "Server=localhost\\SQLEXPRESS; Database=DBTest; Trusted_Connection=True";
                using (mySQLConnection = new SqlConnection(StringConnessione))
                {
                    mySQLConnection.Open();
                    using (SqlCommand mySQLcommand = new SqlCommand("SELECT * FROM TDipendenti", mySQLConnection))
                    {
                        using (SqlDataReader reader = mySQLcommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Dipendente myDipendente = new Dipendente();

                                //compilo myDipendente e poi lo aggiungo a my list articoli
                                myDipendente.DipendenteID = Convert.ToInt32(reader["DipendenteID"]);
                                myDipendente.Cognome = Convert.ToString(reader["Cognome"]);
                                myDipendente.Nome = Convert.ToString(reader["Nome"]);
                                myDipendente.DataNascita = Convert.ToDateTime(reader["DataNascita"]);
                                myDipendente.Sesso = Convert.ToString(reader["Sesso"]);
                                myDipendente.ComuneNascita = Convert.ToString(reader["ComuneNascita"]);
                                myDipendente.ProvinciaNascita = Convert.ToString(reader["ProvinciaNascita"]);
                                myDipendente.Email = Convert.ToString(reader["Email"]);

                                //myDipendente può essere aggiunto alla lista degli articoli
                                myListDipendenti.Add(myDipendente);
                            }
                        }
                    }
                }
            }
            catch
            {
                //le eccezzioni vanno implementate nel frontend tramite un altro metodo
            }
            finally
            {

                //che tutto sia ok o ko il codice dentro finally verrà sempre eseguito
                //chiudo la connessione al db 
                if (mySQLConnection != null && mySQLConnection.State == ConnectionState.Open)
                {
                    mySQLConnection.Close();
                }
            }

            //fine della funzione GetListDipendenti
            return myListDipendenti;
        }
        #endregion LISTA DIPENDENTI


        //2A CHIAMATA
        #region SINGOLO DIPENDENTE
        [Route("Singolo Dipedente")]
        [HttpGet]
        public ResponseDipendente GetDipendente(Int32 DipendenteID)
        {
            ResponseDipendente myResponseDipendente = new ResponseDipendente();

            SqlConnection? mySQLConnection = null;
            try
            {
                String StringConnessione = "Server=localhost\\SQLEXPRESS; Database=DBTest; Trusted_Connection=True";
                mySQLConnection = new SqlConnection(StringConnessione);
                mySQLConnection.Open();
                SqlCommand mySQLCommand = new SqlCommand();
                mySQLCommand.Connection = mySQLConnection;
                mySQLCommand.Parameters.AddWithValue("@DipendenteID", DipendenteID);
                mySQLCommand.CommandText = "SELECT * FROM TDipendenti WHERE DipendenteID=@DipendenteID";
                SqlDataReader reader = mySQLCommand.ExecuteReader();
                if (reader.HasRows == true)
                {
                    Dipendente myDipendente = new Dipendente();
                    while (reader.Read())
                    {
                        myDipendente.DipendenteID = Convert.ToInt32(reader["DipendenteID"]);
                        myDipendente.Cognome = reader["Cognome"] + "";
                        myDipendente.Nome = reader["Nome"] + "";
                        myDipendente.DataNascita = Convert.ToDateTime(reader["DataNascita"] + "");
                        myDipendente.Sesso = reader["Sesso"] + "";
                        myDipendente.ComuneNascita = reader["ComuneNascita"] + "";
                        myDipendente.ProvinciaNascita = reader["ProvinciaNascita"] + "";
                        myDipendente.Email = reader["Email"] + "";
                    }
                    reader.Close();
                    myResponseDipendente.myDipendente = myDipendente;
                    myResponseDipendente.Messaggio = "Ok";
                }
                else
                {
                    myResponseDipendente.myDipendente = null;
                    myResponseDipendente.Messaggio = "Dipedente non presente";
                }
                if (reader.IsClosed == false)
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                myResponseDipendente.Messaggio = ex.Message;
            }
            finally
            {
                if (mySQLConnection != null && mySQLConnection.State == ConnectionState.Open)
                {
                    mySQLConnection.Close();
                }
            }
            return myResponseDipendente;
        }
        #endregion SINGOLO DIPENDENTE


        //2A CHIAMATA (NON UTILIZZATA)
        #region NON UTILIZZATA

        /*[Route("singolodipendente")]
        [HttpGet]

        public ResponseDipendente GetDipendente(RequestDipendente myRequestDipendente)
        {

            SqlConnection? mySQLConnection = null;


            ResponseDipendente myResponseDipendente = new ResponseDipendente();
            string message = " ";
            string query = "SELECT * FROM TDipendenti WHERE ArticoloID=@ArticoloID";

            try
            {
                String StringConnessione = "Server=localhost\\SQLEXPRESS; Database=DBTest; Trusted_Connection=True";
                using (mySQLConnection = new SqlConnection(StringConnessione))
                {
                    mySQLConnection.Open();

                    //comando SQL che seleziona articoli che hanno lo stesso id inserito nel textbox 
                    using (SqlCommand mySQLcommand = new SqlCommand(query, mySQLConnection))
                    {

                        //aggiungo il parametro dell' ArticoloId 
                        mySQLcommand.Parameters.AddWithValue("@DipendenteID", myRequestDipendente.DipendenteID);

                        //faccio svolgere la query al db
                        mySQLcommand.CommandText = query;

                        //leggo i parametri inseriti con data reader
                        using (SqlDataReader reader = mySQLcommand.ExecuteReader())
                        {
                            //se la console legge restituisce lo stato dell'operazione "eseguita" e posso vedere la lista di arrticoli
                            if (reader.Read())
                            {
                                reader.Close();
                                message = "DipendenteID corretto";
                                myResponseDipendente.myDipendente = GetDipendente();
                            }
                            //altrimenti lo stato dell'operazione sarà fallito e la lista di articoli sarà nulla
                            else
                            {
                                message = "Token non corretto";
                                myResponseDipendente.myDipendente = null;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                message = "Eccezione: " + ex.Message;
            }
            finally
            {
                if (mySQLConnection != null && mySQLConnection.State == ConnectionState.Open)
                {
                    mySQLConnection.Close();
                }
            }
            //la funzione mi restituisce la variabile myResponseListaArticoli
            myResponseDipendente.Messaggio = message;
            return myResponseDipendente;
        }*/
        #endregion NON UTILIZZATA


        //3A CHIAMATA
        #region INSERISCI DIPENDENTE
        [Route("Inserisci Dipendente")]
        [HttpPost]

        public String InserisciDipendente(Dipendente myDipendente)
        {
            //codice che fa un insert di dipendente in TDipendenti
            //se tutto va bene --> return "OK"
            //se c'è eccezzione --> return exc.Message


            SqlConnection? mySQLconnection = null;
            string Ritorno = "Dipendente Aggiunto";
            try
            {
                String StringConnessione = "Server=localhost\\SQLEXPRESS; Database=DBTest; Trusted_Connection=True";
                mySQLconnection = new SqlConnection(StringConnessione);
                mySQLconnection.Open();

                SqlCommand mySQLcommand = new SqlCommand();
                mySQLcommand.Connection = mySQLconnection;

                mySQLcommand.Parameters.AddWithValue("@Cognome", myDipendente.Cognome);

                mySQLcommand.Parameters.AddWithValue("@Nome", myDipendente.Nome);

                mySQLcommand.Parameters.AddWithValue("@DataNascita", myDipendente.DataNascita);

                mySQLcommand.Parameters.AddWithValue("@Sesso", myDipendente.Sesso);

                mySQLcommand.Parameters.AddWithValue("@ComuneNascita", myDipendente.ComuneNascita);

                mySQLcommand.Parameters.AddWithValue("@ProvinciaNascita", myDipendente.ProvinciaNascita);

                mySQLcommand.Parameters.AddWithValue("@Email", myDipendente.Email);


                mySQLcommand.CommandText = "INSERT INTO TDipendenti (Cognome, Nome, DataNascita, Sesso, ComuneNascita, ProvinciaNascita, Email)" +
                " VALUES (@Cognome,@Nome,@DataNascita,@Sesso,@ComuneNascita,@ProvinciaNascita,@Email)";

                SqlDataReader reader = mySQLcommand.ExecuteReader();
                if ((!String.IsNullOrEmpty(myDipendente.Nome) && !String.IsNullOrEmpty(myDipendente.Cognome) && !String.IsNullOrEmpty(myDipendente.Sesso) && (myDipendente.Sesso.ToUpper() == "M" || myDipendente.Sesso.ToUpper() == "F") && !String.IsNullOrEmpty(myDipendente.Email) && !String.IsNullOrEmpty(myDipendente.ComuneNascita) && !String.IsNullOrEmpty(myDipendente.ProvinciaNascita)))
                {
                    reader.Close();
                    Ritorno = "Dipendente Aggiunto";
                }
                else
                {
                    Ritorno = " Inserisci tutti i dati o insersci i dati corretti";
                }
            }
            catch (Exception ex)
            {
                Ritorno = ex.Message;
            }
            finally
            {

                if (mySQLconnection != null && mySQLconnection.State == ConnectionState.Open)
                {
                    mySQLconnection.Close();
                }
            }
            return Ritorno;
        }
        #endregion INSERISCI DIPENDENTE

         
        //4A CHIAMATA 
        #region ELIMINA DIPENDENTE
        [Route("Elimina Dipendente")]
        [HttpDelete]

        public String DeleteDipendente(Int32 DipendenteID)
        {
            //codice che fa delete from...
            //elimina 1 solo dipendente che ha DipendenteID= quello ricevuto
            //se tutto va bene -> return ok
            // se c'è eccezione -> return exc.Message

            SqlConnection? mySQLconnection = null;
            string Ritorno = "Dipendente eliminato con successo";
            string RitornoNonEsistente = "Dipendente non esistente";

            try
            {
                String StringConnessione = "Server=localhost\\SQLEXPRESS; Database=DBTest; Trusted_Connection=True";
                using (mySQLconnection = new SqlConnection(StringConnessione))
                {
                    mySQLconnection.Open();

                    int dipendenteIDToDelete = Convert.ToInt32(DipendenteID);

                    // Verifica se DipendenteID esiste
                    using (SqlCommand checkCommand = new SqlCommand("SELECT 1 FROM TDipendenti WHERE DipendenteID = @DipendenteID", mySQLconnection))
                    {
                        checkCommand.Parameters.AddWithValue("@DipendenteID", dipendenteIDToDelete);

                        using (SqlDataReader reader = checkCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // DipendenteID esiste, esegui l'eliminazione
                                reader.Close();

                                using (SqlCommand deleteCommand = new SqlCommand("DELETE FROM TDipendenti WHERE DipendenteID=@DipendenteID", mySQLconnection))
                                {
                                    deleteCommand.Parameters.AddWithValue("@DipendenteID", dipendenteIDToDelete);
                                    deleteCommand.ExecuteNonQuery();
                                }
                            }
                            else 
                            {
                                // DipendenteID non esiste
                                return RitornoNonEsistente;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Ritorno = ex.Message;
            }
            finally
            {
                if (mySQLconnection != null && mySQLconnection.State == ConnectionState.Open)
                {
                    mySQLconnection.Close();
                }
            }
            return Ritorno;
        }
        #endregion ELIMINA DIPENDENTE


        //5A CHIMATA
        #region AGGIORNA EMAIL
        [Route("Aggiorna Email")]
        [HttpPut]

        public String AggiornamentoEmail(Int32 DipendenteID, string Email)
        {
            SqlConnection? mySQLconnection = null;

            Int32 dipendenteIDtoupdate = Convert.ToInt32(DipendenteID); //variabile che definisce l'articolo dal parametro ArticoloID
            string emailToUpdate = Convert.ToString(Email); //variabile che definisce la giacenza dal parametro Quantità
            string Ritorno = " "; //variabile che restituisce il valore della funzione
            try
            {

                //mi connetto al database
                String StringConnessione = "Server=localhost\\SQLEXPRESS; Database=DBTest; Trusted_Connection=True";
                using (mySQLconnection = new SqlConnection(StringConnessione))
                {
                    mySQLconnection.Open();

                    //seleziono il dipendente che voglio modificare 
                    using (SqlCommand mySQLcommand = new SqlCommand("SELECT DipendenteID from TDipendenti WHERE DipendenteID=@DipendenteID", mySQLconnection))
                    {

                        //aggiunge il parametro da modificare
                        mySQLcommand.Parameters.AddWithValue("@DipendenteID", dipendenteIDtoupdate);

                        using (SqlDataReader reader = mySQLcommand.ExecuteReader())
                        {

                            //se il dipendente c'è
                            if (reader.Read() && Email.Contains('@') == true)
                            {
                                reader.Close();

                                using (SqlCommand mySQLcommand2 = new SqlCommand("UPDATE TDipendenti SET Email=@Email WHERE DipendenteID=@DipendenteID", mySQLconnection))
                                {
                                    mySQLcommand2.Parameters.AddWithValue("@Email", emailToUpdate);
                                    mySQLcommand2.Parameters.AddWithValue("@DipendenteID", dipendenteIDtoupdate);

                                    mySQLcommand2.ExecuteNonQuery();
                                    Ritorno = "Email aggiornata correttamente";
                                }
                            }
                            //altrimenti il dipendente non esiste 
                            else
                            {
                                // DipendenteID non esiste, puoi gestire questo caso se necessario
                                Ritorno = "DipendenteID non esistente o formato email non corretto";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Ritorno = ex.Message;
            }
            finally
            {
                if (mySQLconnection != null && mySQLconnection.State == ConnectionState.Open)
                {
                    mySQLconnection.Close();
                }
            }
            //rimandami al risultato della funzione 
            return Ritorno;
        }
        #endregion AGGIORNA EMAIL


        //6A CHIAMATA
        #region CODICE FISCALE

        #region CHIAMATA
        [Route("Codice Fiscale Dipendente")]
        [HttpGet]
        public string GetCodiceFiscale(int DipendenteID)
        {
            SqlConnection? mySQLconnection = null;

            string mess = "", catasto = "";
            ResponseDipendente resp = new ResponseDipendente();
            resp.Messaggio = "";
            try
            {
                resp = GetDipendente(DipendenteID);
                if (resp.Messaggio == "Ok")
                {
                    String StringConnessione = "Server=localhost\\SQLEXPRESS; Database=DBTest; Trusted_Connection=True";

                    using (mySQLconnection = new SqlConnection(StringConnessione))
                    {
                        mySQLconnection.Open();

                        using (SqlCommand mySQLcommand = new SqlCommand("SELECT CodiceCatastale AS CC FROM TComuni " +
                        "WHERE ComuneNascita LIKE @ComuneNascita AND ProvinciaNascita LIKE @ProvinciaNascita", mySQLconnection))
                        {
                            mySQLcommand.Parameters.AddWithValue("@ComuneNascita", resp.myDipendente.ComuneNascita.ToUpper());
                            mySQLcommand.Parameters.AddWithValue("@ProvinciaNascita", resp.myDipendente.ProvinciaNascita.ToUpper());

                            using (SqlDataReader reader = mySQLcommand.ExecuteReader())
                            {

                                if (reader.HasRows== true) 
                                {
                                    while (reader.Read()) 
                                    {
                                        catasto = reader["CC"] + "";
                                    }
                                    reader.Close();

                                    string anno = resp.myDipendente.DataNascita.Year.ToString().Substring(2);
                                    string cod = CodCognome(resp.myDipendente) + CodNome(resp.myDipendente) + anno + LetteraMese(resp.myDipendente) + NascitaSesso(resp.myDipendente) + catasto;
                                    cod += CodiceControllo(cod);
                                    mess = cod;
                                }
                                else
                                {
                                    mess = "Catasto non presente";
                                    if(reader.IsClosed == false) 
                                    {
                                        reader.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mess = ex.Message;
                resp.Messaggio = mess;
            }
            if (resp.Messaggio == "Ok")
                resp.Messaggio = mess;

            return resp.Messaggio;
        }
        #endregion CHIAMATA

        #region METODO CODCOGNOME
        private string CodCognome(Dipendente d)
        {
            string vocali = "aeiou";
            string cod = d.Cognome.ToLower(), voc_cogn = "";
            //doppio ciclo for
            for (int i = 0; i < d.Cognome.Length; i++)
            {
                for (int j = 0; j < vocali.Length; j++)
                {
                    if (d.Cognome.ToLower()[i] == vocali[j])    //controllo che d.Cognome[i] sia uguale a vocali[j]
                    {
                        voc_cogn += vocali[j];      //aggiungo la vocale a voc_cogn in caso servisse successivamente
                        cod = cod.Replace(vocali[j].ToString(), "");    //"cancello" la vocale
                    }
                }
            }
            cod = cod.Replace(" ", "");     //tolgo gli spazi
            if (cod.Length < 3)     //se la lunghezza di cod è < 3, aggiungo le vocali
            {
                cod += voc_cogn;
                switch (cod.Length)
                {
                    case 1:
                        cod += "XX";
                        break;
                    case 2:
                        cod += "X";
                        break;
                }
            }
            cod = cod.Substring(0, 3); //riduco la lunghezza di cod
            return cod.ToUpper();
        }
        #endregion METODO CODCOGNOME

        #region METODO CODNOME
        private string CodNome(Dipendente d)
        {
            string vocali = "aeiou";
            string cod = d.Nome.ToLower(), voc_nome = "";
            //doppio ciclo for
            for (int i = 0; i < d.Nome.Length; i++)
            {
                for (int j = 0; j < vocali.Length; j++)
                {
                    if (d.Nome.ToLower()[i] == vocali[j])    //controllo che d.Nome[i] sia uguale a vocali[j]
                    {
                        voc_nome += vocali[j];      //aggiungo la vocale a voc_nome in caso servisse successivamente
                        cod = cod.Replace(vocali[j].ToString(), "");    //"cancello" la vocale
                    }
                }
            }
            cod = cod.Replace(" ", "");     //tolgo gli spazi
            if (cod.Length <= 3)     //se la lunghezza di cod è <= 3, aggiungo le vocali
            {
                cod += voc_nome;
                switch (cod.Length)
                {
                    case 1:
                        cod += "XX";
                        break;
                    case 2:
                        cod += "X";
                        break;
                }
                cod = cod.Substring(0, 3);
            }
            else if (cod.Length > 3)     //se la lunghezza di cod è < 3, prendo la prima, la terza e la quarta consonante
                cod = cod[0] + "" + cod[2] + "" + cod[3];
            return cod.ToUpper();
        }
        #endregion METODO CODNOME

        #region METODO LETTERAMESE
        private string LetteraMese(Dipendente d)
        {
            string lettera = "";
            switch (d.DataNascita.Month)
            {
                case 01:
                    lettera = "A";
                    break;
                case 02:
                    lettera = "B";
                    break;
                case 03:
                    lettera = "C";
                    break;
                case 04:
                    lettera = "D";
                    break;
                case 05:
                    lettera = "E";
                    break;
                case 06:
                    lettera = "H";
                    break;
                case 07:
                    lettera = "L";
                    break;
                case 08:
                    lettera = "M";
                    break;
                case 09:
                    lettera = "P";
                    break;
                case 10:
                    lettera = "R";
                    break;
                case 11:
                    lettera = "S";
                    break;
                case 12:
                    lettera = "T";
                    break;
            }
            return lettera;
        }
        #endregion LETTERAMESE

        #region METODO NASCITASESSO
        private string NascitaSesso(Dipendente d)
        {
            int val = d.DataNascita.Day;
            if (d.Sesso.ToLower() == "femmina")
                val += 40;

            string codice = val + "";
            if (d.Sesso.ToLower() == "maschio")
            {
                if (d.DataNascita.Day < 10)
                    codice = "0" + val;
            }
            return codice;
        }
        #endregion METODO NASCITASESSO

        #region METODO CODICECONTROLLO
        private string CodiceControllo(string codice)
        {
            int controllo = 0;
            for (int i = 0; i < codice.Length; i++)
            {
                switch (codice[i].ToString())
                {
                    case "A":
                    case "0":
                        if (i % 2 == 0)
                            controllo += 1;
                        else
                            controllo += 0;
                        break;
                    case "B":
                    case "1":
                        if (i % 2 == 0)
                            controllo += 0;
                        else
                            controllo += 1;
                        break;
                    case "C":
                    case "2":
                        if (i % 2 == 0)
                            controllo += 5;
                        else
                            controllo += 2;
                        break;
                    case "D":
                    case "3":
                        if (i % 2 == 0)
                            controllo += 7;
                        else
                            controllo += 3;
                        break;
                    case "E":
                    case "4":
                        if (i % 2 == 0)
                            controllo += 9;
                        else
                            controllo += 4;
                        break;
                    case "F":
                    case "5":
                        if (i % 2 == 0)
                            controllo += 13;
                        else
                            controllo += 5;
                        break;
                    case "G":
                    case "6":
                        if (i % 2 == 0)
                            controllo += 15;
                        else
                            controllo += 6;
                        break;
                    case "H":
                    case "7":
                        if (i % 2 == 0)
                            controllo += 17;
                        else
                            controllo += 7;
                        break;
                    case "I":
                    case "8":
                        if (i % 2 == 0)
                            controllo += 19;
                        else
                            controllo += 8;
                        break;
                    case "J":
                    case "9":
                        if (i % 2 == 0)
                            controllo += 21;
                        else
                            controllo += 9;
                        break;
                    case "K":
                        if (i % 2 == 0)
                            controllo += 2;
                        else
                            controllo += 10;
                        break;
                    case "L":
                        if (i % 2 == 0)
                            controllo += 4;
                        else
                            controllo += 11;
                        break;
                    case "M":
                        if (i % 2 == 0)
                            controllo += 18;
                        else
                            controllo += 12;
                        break;
                    case "N":
                        if (i % 2 == 0)
                            controllo += 20;
                        else
                            controllo += 13;
                        break;
                    case "O":
                        if (i % 2 == 0)
                            controllo += 11;
                        else
                            controllo += 14;
                        break;
                    case "P":
                        if (i % 2 == 0)
                            controllo += 3;
                        else
                            controllo += 15;
                        break;
                    case "Q":
                        if (i % 2 == 0)
                            controllo += 6;
                        else
                            controllo += 16;
                        break;
                    case "R":
                        if (i % 2 == 0)
                            controllo += 8;
                        else
                            controllo += 17;
                        break;
                    case "S":
                        if (i % 2 == 0)
                            controllo += 12;
                        else
                            controllo += 18;
                        break;
                    case "T":
                        if (i % 2 == 0)
                            controllo += 14;
                        else
                            controllo += 19;
                        break;
                    case "U":
                        if (i % 2 == 0)
                            controllo += 16;
                        else
                            controllo += 20;
                        break;
                    case "V":
                        if (i % 2 == 0)
                            controllo += 10;
                        else
                            controllo += 21;
                        break;
                    case "W":
                        if (i % 2 == 0)
                            controllo += 22;
                        else
                            controllo += 22;
                        break;
                    case "X":
                        if (i % 2 == 0)
                            controllo += 25;
                        else
                            controllo += 23;
                        break;
                    case "Y":
                        if (i % 2 == 0)
                            controllo += 24;
                        else
                            controllo += 24;
                        break;
                    case "Z":
                        if (i % 2 == 0)
                            controllo += 23;
                        else
                            controllo += 25;
                        break;
                    default:
                        controllo += 0;
                        break;
                }
            }
            controllo %= 26;
            return Convert.ToChar(controllo + 65).ToString();
        }
        #endregion METODO CODICECONTROLLO

        #endregion CODICE FISCALE

    }
}

