using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Helpers
{
    public class User
    {
        private static User _instance;

        public string Name { get; set; }
        public int Score { get; private set; } = 0;

        public static User GetInstance()
        {
            if (_instance == null)
            {
                _instance = new User();
            }

            return _instance;
        }

        public void IncreaseScore(int value)
        {
            Score += value;
        }

        public void ResetScore()
        {
            Score = 0;
        }
    }
}
