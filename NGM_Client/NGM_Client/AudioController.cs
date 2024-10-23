using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
namespace NGM_Client
{
    class AudioController
    {



        int micNumber = 0;
        int delay;

        public AudioController(int _micNumber, int _delay)
        {
            micNumber = _micNumber;
            delay = _delay;
        }


    }
}