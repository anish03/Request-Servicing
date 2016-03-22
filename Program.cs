using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ConsoleApplication
{
    public interface IServiceRequest
    {
        void SendRequest(string desc, string status);
    }

    public interface IAdminRequest
    {
        void ServiceRequest();
    }

    class Admin : IAdminRequest
    {

        public void ServiceRequest()
        {
            try
            {
                string strConnection = ConfigurationManager.ConnectionStrings["serviceDB"].ConnectionString;

                string strSelect = "SELECT * FROM SERVICEREQ";

                SqlDataAdapter serviceADP = new SqlDataAdapter(strSelect, strConnection);
                SqlCommandBuilder builder = new SqlCommandBuilder(serviceADP);

                DataSet DS = new DataSet("Servicing");
                serviceADP.Fill(DS, "service");

                int cur_row = 0;

                foreach(DataRow row in DS.Tables["service"].Rows)
                {
                    if (row["REQ_CLOSED_TIME"] != null)   
                    {
                        Console.WriteLine(row["ID"]+"\t"+row["REQ_OPEN_TIME"]+"\t"+row["REQ_DESC"]+"\t"+row["STAT"]+"\t"+row["REQ_CLOSED_TIME"]);
                        Console.WriteLine("\nDo you wish to service this request?(Press 0 to service)\n");
                        int option = Convert.ToInt32(Console.ReadLine());
                        if (option == 0)
                        {
                            DS.Tables[0].Rows[cur_row][3] = "Closed";
                        }
                    }
                    cur_row++;
                }

                serviceADP.Update(DS, "service");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    class User : IServiceRequest
    {
        private int user_id;

        public int User_ID
        {
            get
            {
                return user_id;
            }
            set
            {
                user_id = value;
            }
        }

        public User(int id)
        {
            User_ID = id;
        }

        public void Create_Request()
        {
            Console.WriteLine(DateTime.Now.Date);
            Console.WriteLine("Enter Service Request:\n");
            string request_desc = Convert.ToString(Console.ReadLine());
            SendRequest(request_desc, "Open");
        }


        public void SendRequest(string desc, string status)
        {
            try
            {
                string strConnection = ConfigurationManager.ConnectionStrings["serviceDB"].ConnectionString;

                string insertREQ = "INSERT SERVICEREQ (REQ_DESC,STAT) VALUES(@DESC,@STATUS)";

                using (SqlConnection connection = new SqlConnection(strConnection))
                {
                    SqlCommand sendREQ = new SqlCommand(insertREQ, connection);


                    SqlParameter Service_Desc = new SqlParameter("@DESC", SqlDbType.VarChar);
                    Service_Desc.Value = desc;

                    SqlParameter Service_Stat = new SqlParameter("@STATUS", SqlDbType.VarChar);
                    Service_Stat.Value = status;

                    sendREQ.Parameters.Add(Service_Desc);
                    sendREQ.Parameters.Add(Service_Stat);
                    
                    connection.Open();

                    int rows = sendREQ.ExecuteNonQuery();
                    Console.WriteLine("{0} rows affected",rows);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            User u1 = new User(1);
            //u1.Create_Request();

            Admin a1 = new Admin();
            a1.ServiceRequest();
        }
    }
}
