using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace WinFormsFaultTolChat
{
    public class Client 
    {
        private Thread ReceiveThread;
        private Thread decodeThread;
        private UdpClient SendClient = new UdpClient(1996);
        private string MessageSender;
        private int SendNumber;
        //the message number that after each message,iterates upwards
        private int MessageNumber = 0;
        //the last  message number received 
        private int LastMessage;
        private List<string> messages = new List<string>();
        private bool clientbool = true;

        public List<string> getmessages()
        {
            return messages;
        }

        public void Closelisten()
        {
            clientbool = false;
            Thread.Sleep(400);
            SendClient.Close();
            
        }

        /// <summary>
        /// encodes a message byte array with the Hamming(7,4) scheme, as the challenge states that each byte will be corrupted.
        /// this per byte corruption stops us from using a hamming(12,8) scheme, as that means half a byte is shared with another data byte, which could cause irreparable corruption.
        /// </summary>
        /// <returns> a byte array encoded with Hamming(7,4)</returns>
        private Byte[] HammingEncodeMessage(Byte[] byteblock)
        {
            Byte[] hammingBlock = new Byte[(byteblock.Length * 2)];
            for (int i = 0; i < byteblock.Length; i++)
            {
                Byte[] posthamming = hammingBytes(byteblock[i]);
                posthamming.CopyTo(hammingBlock, (i * 2));
            }
            return hammingBlock;
        }

        /// <summary>
        /// encodes a opcode byte  with the Hamming(7,4) scheme, as the challenge states that each byte will be corrupted.
        /// this per byte corruption stops us from using a hamming(12,8) scheme, as that means half a byte is shared with another byte, which could cause irreparable corruption.
        /// </summary>
        /// <returns>  a 2 byte opcode array encoded with Hamming(7,4)</returns>
        private byte[] HammingEncodeOpcode(byte byteblock)
        {
            byte[] hammingBlock = hammingBytes(byteblock);
            return hammingBlock;
        }

        /// <summary>
        /// the actual hamming scheme, using a basic matrix multiplication scheme
        /// </summary>
        /// <param name="target">the byte that will be encoded</param>
        /// <returns>returns the two bytes created from the hamming(7,4) encoding scheme</returns>
        private byte[] hammingBytes(byte target)
        {
            //the generator matrix used to multiply our 4 data bits for hamming encoding
            //first three columns are parity bits, last 4 are data bits
            //TODO: May need to flip dimensions on this matrix https://en.wikipedia.org/wiki/Hamming(7,4)
            Int16[][] HammingMatrix =
            {
                new Int16[] {1, 1, 0, 1},
                new Int16[] {1, 0, 1, 1},
                new Int16[] {1, 0, 0, 0},
                new Int16[] {0, 1, 1, 1},
                new Int16[] {0, 1, 0, 0},
                new Int16[] {0, 0, 1, 0},
                new Int16[] {0, 0, 0, 1}
            };
            BitArray OGbits = new BitArray(new Byte[] { target });
            Console.WriteLine(OGbits.Count);
            //original data bits converted to int16
            Int16[][] firsthalf =             {
                new Int16[] {Convert.ToInt16(OGbits[0])},
                new Int16[] {Convert.ToInt16(OGbits[1])},
                new Int16[] {Convert.ToInt16(OGbits[2])},
                new Int16[] {Convert.ToInt16(OGbits[3])}
            };
            Int16[][] secondhalf =             {
                new Int16[] {Convert.ToInt16(OGbits[4])},
                new Int16[] {Convert.ToInt16(OGbits[5])},
                new Int16[] {Convert.ToInt16(OGbits[6])},
                new Int16[] {Convert.ToInt16(OGbits[7])}
            };
            // final arrays for each set of 4 bits (7 hamming encoded bool "bits" + 1 null "bit" at end)
            bool[] boolfirst = new bool[8];
            bool[] boolsecond = new bool[8];
            //use the generator matrix to create an int version of the finished product 
            for (int i = 0; i < 7; i++)
            {
                int temp = 0;
                int temp2 = 0;
                for (int j = 0; j < 4; j++)
                {
                    //TODO: ensure this is the correct way for the hamming matrix to multiply by
                    temp +=  HammingMatrix[i][j]*firsthalf[j][0];
                    temp2 +=  HammingMatrix[i][j]* secondhalf[j][0];
                }
                //these are the bool converting if statements for the first half of the byte
                //if the final number is even, set the bit to "0"
                if (temp % 2 == 0)
                {
                    boolfirst[i] = false;
                }
                //if the final number is odd, set the bit to "1"
                else if (temp % 2 != 0)
                {
                    boolfirst[i] = true;
                }

                //these are the bool converting if statements for the second half of the byte
                //if the final number is even, set the bit to "0"
                if (temp2 % 2 == 0)
                {
                    boolsecond[i] = false;
                }
                //if the final number is odd, set the bit to "1"
                else if (temp2 % 2 != 0)
                {
                    boolsecond[i] = true;
                }
            }
            boolfirst[7] = false;
            boolsecond[7] = false;
            Byte finalfirst = Encodebyte(boolfirst);
            Byte finalsecond = Encodebyte(boolsecond);
            return new byte[2] { finalfirst, finalsecond };
        }

        /// <summary>
        /// encapsulates the parity check, error correction, and decoding of two encoded bytes, then forms one full data byte from them
        /// </summary>
        /// <param name="firsthalf">the first encoded byte, and soon to be the first half of a decoded byte</param>
        /// <param name="secondhalf">the second encoded byte, and soon to be the second half of a decoded byte</param>
        /// <returns> one decoded byte</returns>
        public Byte hammingdecode(Byte firsthalf, Byte secondhalf)
        {
            BitArray OGbits = new BitArray(new Byte[] { (byte)'a' });
            bool[] test = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                if (OGbits[(7-i)] == true)
                {
                    test[(i)] = true;
                }
                else test[(i)] = false;
            }

            byte test2 = Encodebyte(test);
            Console.WriteLine(test2);
            //first we check the parity
            //we create the parity matrix which will be multiplied with our data to find if there is errors 
            Int16[][] ParityMatrix =
            {
                new Int16[] {1,0,1,0,1,0,1},
                new Int16[] {0,1,1,0,0,1,1},
                new Int16[] {0,0,0,1,1,1,1}
            };
            //we then create a column jagged array for each of the bytes,
            //and remove the spacer bit at the end
            BitArray bitfirst = new BitArray(new Byte[] { firsthalf });
            BitArray bitsecond = new BitArray(new Byte[] { secondhalf });


            Int16[][] jaggedfirst =
            {
                new Int16[] {Convert.ToInt16(bitfirst[7])},
                new Int16[] {Convert.ToInt16(bitfirst[6])},
                new Int16[] {Convert.ToInt16(bitfirst[5])},
                new Int16[] {Convert.ToInt16(bitfirst[4])},
                new Int16[] {Convert.ToInt16(bitfirst[3])},
                new Int16[] {Convert.ToInt16(bitfirst[2])},
                new Int16[] {Convert.ToInt16(bitfirst[1])}
            };
            Int16[][] jaggedsecond =
            {
                new Int16[] {Convert.ToInt16(bitsecond[7])},
                new Int16[] {Convert.ToInt16(bitsecond[6])},
                new Int16[] {Convert.ToInt16(bitsecond[5])},
                new Int16[] {Convert.ToInt16(bitsecond[4])},
                new Int16[] {Convert.ToInt16(bitsecond[3])},
                new Int16[] {Convert.ToInt16(bitsecond[2])},
                new Int16[] {Convert.ToInt16(bitsecond[1])}
            };
            //the bit check arrays
            bool[] firstcheck = new bool[3];
            bool[] secondcheck = new bool[3];
            //use the parity matrix to determine if the byte has maintained integrity
            for (int i = 0; i < 3; i++)
            {
                int temp = 0;
                int temp2 = 0;
                for (int j = 0; j < 7; j++)
                {
                    //TODO: ensure this is the correct way for the hamming matrix to multiply by
                    temp += ParityMatrix[i][j] * jaggedfirst[j][0];
                    temp2 += ParityMatrix[i][j] * jaggedsecond[j][0];
                }
                //these are the bool converting if statements for the first half of the byte
                //if the final number is even, set the bit to "0"
                if (temp % 2 == 0)
                {
                    firstcheck[i] = false;
                }
                //if the final number is odd, set the bit to "1"
                else if (temp % 2 != 0)
                {
                    firstcheck[i] = true;
                }

                //these are the bool converting if statements for the second half of the byte
                //if the final number is even, set the bit to "0"
                if (temp2 % 2 == 0)
                {
                    secondcheck[i] = false;
                }
                //if the final number is odd, set the bit to "1"
                else if (temp2 % 2 != 0)
                {
                    secondcheck[i] = true;
                }
            }
            //the integers that will keep track of which bit is incorrect
            int firsterror = 0;
            int seconderror = 0;
            if (firstcheck[0] == true) firsterror += 1;
            if (firstcheck[1] == true) firsterror += 2;
            if (firstcheck[2] == true) firsterror += 4;
            //the second check is now calculated
            if (secondcheck[0] == true) seconderror += 1;
            if (secondcheck[1] == true) seconderror += 2;
            if (secondcheck[2] == true) seconderror += 4;
            //if  the resulting arrays have anything but all zeros, we flip whatever bit the check arrays point to
            if (firsterror != 0)
            {
                if (jaggedfirst[firsterror-1][0] == 1)
                {
                    jaggedfirst[firsterror-1][0] = 0;
                }
                else
                {
                    jaggedfirst[firsterror-1][0] = 1;
                }
            }
            if (seconderror != 0)
            {
                if (jaggedsecond[seconderror-1][0] == 1)
                {
                    jaggedsecond[seconderror-1][0] = 0;
                }
                else
                {
                    jaggedsecond[seconderror-1][0] = 1;
                }
            }
            //the pre boolean byte
            Int16[] prebool = new Int16[8];
            prebool[0] = jaggedfirst[2][0];
            prebool[1] = jaggedfirst[4][0];
            prebool[2] = jaggedfirst[5][0];
            prebool[3] = jaggedfirst[6][0];
            prebool[4] = jaggedsecond[2][0];
            prebool[5] = jaggedsecond[4][0];
            prebool[6] = jaggedsecond[5][0];
            prebool[7] = jaggedsecond[6][0];
            //the pre byte boolean array
            bool[] prebyte = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                if (prebool[7-i] == 1) prebyte[i] = true;
                else prebyte[i] = false;

            }

            byte finishedbyte = Encodebyte(prebyte);
            return finishedbyte;

        }

        /// <summary>
        /// takes one of the boolean arrays made from our HammingBytes function, then converts each bool to an actual bit, creating a byte from eight of the bools 
        /// </summary>
        /// <param name="byteTarget"> a boolean array made in the hammingBytes function</param>
        /// <returns>a Byte made from our boolean array </returns>
        private Byte Encodebyte(bool[] byteTarget)
        {
            Byte result = 0;
            int index = 8 - byteTarget.Length;
            foreach (bool bit in byteTarget)
            {
                if (bit == true)
                {
                    result |= (Byte)(1 << (7 - index));
                    
                }
                index++;
            }
            return result;
        }

        /// <summary>
        /// the send function for text messages.
        /// each UDP packet is capped at the minimum MTU size(508 bytes).
        /// if larger, the message is split up into multiple packets
        /// </summary>

        //TODO: create a for loop system for sending multiple UDP packets due to length constraints 
        public void SendEncodedMessage(string Message, string amount)
        {
            string colormessage = "--S" + Message;
            messages.Add(colormessage);
            Byte opcode = (byte)'M';
            Byte sendingnumber = (byte)MessageNumber;
            MessageNumber++;
            //converts the amount of messages needed to send into an int
            int MessagesSent = 0;
            if (!Int32.TryParse(amount, out MessagesSent))
            {
                MessagesSent = 25;
            }
            Byte[] MessageCode = Encoding.ASCII.GetBytes(Message);
            Byte[] numberCode = HammingEncodeOpcode(sendingnumber);
            Byte[] HammingOp = HammingEncodeOpcode(opcode);
            Byte[] HammingMessage = HammingEncodeMessage(MessageCode);
            //TODO: make a chunking loop for messages longer than 504 bytes 
            if (HammingMessage.Length + HammingOp.Length >= 7000)
            {
            }
            //TODO: make two different executables so that they can communicate correctly with corruptor
            else if (HammingMessage.Length + HammingOp.Length <= 7000)
            {
                Byte[] FullMessage = new Byte[(HammingOp.Length + 2 + HammingMessage.Length)];
                HammingOp.CopyTo(FullMessage, 0);
                numberCode.CopyTo(FullMessage, 2);
                HammingMessage.CopyTo(FullMessage, 4);
                //1995 for first version, 1996 for second version: This port will be what messages from the corruptor are sent to 
                //1982 for first verison, 1983 for second version 
                IPEndPoint corruptor = new IPEndPoint(IPAddress.Loopback, 1983);
                int Counter = 0;
                while (Counter < MessagesSent)
                {
                    SendClient.Send(FullMessage, FullMessage.Length, corruptor);
                    Thread.Sleep(20);
                    Counter++;
                }

            }

        }
        //TODO: finish the Receive message function, look at corruptor clients to see how to do it.
        public void receiveEncodedMessage()
        {
            Queue previousmessages = new Queue();
            //TODO: change this when making the second version
            UdpClient ReceiveClient = new UdpClient();
            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Loopback, 1996);
            while (clientbool==true)
            {
                byte[] receivedBytes = SendClient.Receive(ref remoteEndpoint);
                int length = (receivedBytes.Length - 2) / 2;
                byte[] message = new byte[7000];
                int messagecounter = 0;
                int incomingnumber = (int)hammingdecode(receivedBytes[2], receivedBytes[3]);
                if (!previousmessages.Contains(incomingnumber))
                {
                    for (int i = 4; i < receivedBytes.Length; i += 2)
                    {
                        byte decoded = hammingdecode(receivedBytes[i], receivedBytes[i + 1]);
                        message[messagecounter] = decoded;
                        messagecounter++;
                    }

                    Array.Resize(ref message, messagecounter);
                    string bitString = new string(System.Text.Encoding.ASCII.GetString(message).ToCharArray());
                    string colorbitString = "--R" + bitString;
                    messages.Add(colorbitString);
                    previousmessages.Enqueue(incomingnumber);
                }
            }
            ReceiveClient.Close();
            return;
        }

        public void Connect()
        {
            ReceiveThread = new Thread(new ThreadStart(receiveEncodedMessage));
            ReceiveThread.Start();
            ReceiveThread = null;
        }
    }
}
