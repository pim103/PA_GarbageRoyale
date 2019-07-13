using UnityEngine;

namespace GarbageRoyale.Scripts
{
    [System.Serializable]
    public class DataState
    {
        public string userId;
        public string nickName;
        public string role;
        public bool isInMenu;
        public bool endGame;

        public DataState(string userId, string nickName, string role, bool isInMenu, bool endGame)
        {
            this.userId = userId;
            this.nickName = nickName;
            this.role = role;
            this.isInMenu = isInMenu;
            this.endGame = endGame;
        }

        public string UserId
        {
            get => userId;
            set => userId = value;
        }

        public string NickName
        {
            get => nickName;
            set => nickName = value;
        }

        public string Role
        {
            get => role;
            set => role = value;
        }

        public bool IsInMenu
        {
            get => isInMenu;
            set => isInMenu = value;
        }

        public bool EndGame
        {
            get => endGame;
            set => endGame = value;
        }
    }
}