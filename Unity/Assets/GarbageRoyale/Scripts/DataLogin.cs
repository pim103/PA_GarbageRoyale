using UnityEngine;

namespace GarbageRoyale.Scripts
{
    [System.Serializable]
    public class DataLogin
    {
        public string email;

        public DataLogin(string email)
        {
            this.email = email;
        }

        public string Email
        {
            get => email;
            set => email = value;
        }
    }
}