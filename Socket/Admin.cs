using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socket
{
    public interface Iobeservee
    {
        string ID { get; set; }
        string Password { get; set; }
        LogInStatus Stauts { get; set; }
        void LogIn();
        void LogOut();
    }

    public enum LogInStatus
    {
        LOGIN,
        LOGOUT,
        LOGERROR
    }

    public class Admin: Iobeservee
    {
        private string id;
        public string ID
        {
            get { return id; }
            set
            {
                if (value.Length == 5)
                    id = value;
                else
                    throw new ArgumentException("Incorrect number of ID digits");
            }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set
            {
                if (value.Length >= 8)
                    password = value;
                else throw new ArgumentException("Number of password must above 8");
            }
        }

        private LogInStatus status = LogInStatus.LOGOUT;

        public LogInStatus Stauts
        {
            get { return status; }
            set { status = value; }
        }
        public Admin()
        {

        }
        public Admin(string id, string password)
        {
            ID = id;
            Password = password;
        }


        public bool IsLogIn()
        {
            return status == LogInStatus.LOGIN;
        }

        public void LogIn() 
        {
            status = LogInStatus.LOGIN;
        }
        public void LogOut() 
        { 
            status = LogInStatus.LOGOUT;
        }

    }
}
