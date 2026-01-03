using Continuum93.Emulator.CPU;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Continuum93.Emulator.Mnemonics
{
    public static class GenericInitializer
    {
        static byte[] opCodes = new byte[256];
        static string tempReport = "";
        static byte currentInstruction = 0;
        static string currentInstructionString;

        static readonly Dictionary<string, byte> opCodeDefinition = [];

        public static void Initialize()
        {
            InitializeOpCodeDefinition();
            
            InitInstruction("LD", Mnem.LD);
            InitInstruction("ADD", Mnem.ADD);
            // SUB now reuses the shared addressing matrix (same as ADD/LD)
            InitInstruction("SUB", Mnem.SUB);
        }

        private static void InitInstruction(string oper, byte opCode)
        {
            currentInstruction = opCode;
            currentInstructionString = oper;

            PushInstruction("r,n", Instructions._r_n);        //OK
            PushInstruction("r,r", Instructions._r_r);          //OK
            PushInstruction("r,(nnn)", Instructions._r_InnnI);  //OK
            PushInstruction("r,(nnn,nnn)", Instructions._r_Innn_nnnI);      //OK
            PushInstruction("r,(nnn,r)", Instructions._r_Innn_rI);          //OK
            PushInstruction("r,(nnn,rr)", Instructions._r_Innn_rrI);        //OK
            PushInstruction("r,(nnn,rrr)", Instructions._r_Innn_rrrI);      // Duplicate 1      //OK
            PushInstruction("r,(rrr)", Instructions._r_IrrrI);                                  //OK
            PushInstruction("r,(rrr,nnn)", Instructions._r_Irrr_nnnI);      // Duplicate 1      //OK
            PushInstruction("r,(rrr,r)", Instructions._r_Irrr_rI);          //OK
            PushInstruction("r,(rrr,rr)", Instructions._r_Irrr_rrI);        //OK
            PushInstruction("r,(rrr,rrr)", Instructions._r_Irrr_rrrI);      //OK
            PushInstruction("r,fr", Instructions._r_fr);         //         //OK

            PushInstruction("rr,nn", Instructions._rr_nn);                 //OK
            PushInstruction("rr,r", Instructions._rr_r);                    //OK
            PushInstruction("rr,rr", Instructions._rr_rr);                  //OK
            PushInstruction("rr,(nnn)", Instructions._rr_InnnI);            //OK
            PushInstruction("rr,(nnn,nnn)", Instructions._rr_Innn_nnnI);    //OK
            PushInstruction("rr,(nnn,r)", Instructions._rr_Innn_rI);        //OK
            PushInstruction("rr,(nnn,rr)", Instructions._rr_Innn_rrI);      //OK
            PushInstruction("rr,(nnn,rrr)", Instructions._rr_Innn_rrrI);        // Duplicate 2      //OK
            PushInstruction("rr,(rrr)", Instructions._rr_IrrrI);            //OK
            PushInstruction("rr,(rrr,nnn)", Instructions._rr_Irrr_nnnI);        // Duplicate 2      //OK
            PushInstruction("rr,(rrr,r)", Instructions._rr_Irrr_rI);            //OK
            PushInstruction("rr,(rrr,rr)", Instructions._rr_Irrr_rrI);          //OK
            PushInstruction("rr,(rrr,rrr)", Instructions._rr_Irrr_rrrI);        //OK
            PushInstruction("rr,fr", Instructions._rr_fr);        //            //OK

            PushInstruction("rrr,nnn", Instructions._rrr_nnn);          //OK
            PushInstruction("rrr,r", Instructions._rrr_r);              //OK
            PushInstruction("rrr,rr", Instructions._rrr_rr);            //OK
            PushInstruction("rrr,rrr", Instructions._rrr_rrr);          //OK
            PushInstruction("rrr,(nnn)", Instructions._rrr_InnnI);              //OK
            PushInstruction("rrr,(nnn,nnn)", Instructions._rrr_Innn_nnnI);      //OK
            PushInstruction("rrr,(nnn,r)", Instructions._rrr_Innn_rI);          //OK
            PushInstruction("rrr,(nnn,rr)", Instructions._rrr_Innn_rrI);        //OK
            PushInstruction("rrr,(nnn,rrr)", Instructions._rrr_Innn_rrrI);          // Duplicate 3      //OK
            PushInstruction("rrr,(rrr)", Instructions._rrr_IrrrI);                  // OK
            PushInstruction("rrr,(rrr,nnn)", Instructions._rrr_Irrr_nnnI);          // Duplicate 3      // OK
            PushInstruction("rrr,(rrr,r)", Instructions._rrr_Irrr_rI);              // OK
            PushInstruction("rrr,(rrr,rr)", Instructions._rrr_Irrr_rrI);            // OK
            PushInstruction("rrr,(rrr,rrr)", Instructions._rrr_Irrr_rrrI);          // OK
            PushInstruction("rrr,fr", Instructions._rrr_fr);       //       // OK

            PushInstruction("rrrr,nnnn", Instructions._rrrr_nnnn);            // OK
            PushInstruction("rrrr,r", Instructions._rrrr_r);                // OK
            PushInstruction("rrrr,rr", Instructions._rrrr_rr);              // OK
            PushInstruction("rrrr,rrr", Instructions._rrrr_rrr);            // OK
            PushInstruction("rrrr,rrrr", Instructions._rrrr_rrrr);          // OK
            PushInstruction("rrrr,(nnn)", Instructions._rrrr_InnnI);        // OK
            PushInstruction("rrrr,(nnn,nnn)", Instructions._rrrr_Innn_nnnI);            // OK
            PushInstruction("rrrr,(nnn,r)", Instructions._rrrr_Innn_rI);                // OK
            PushInstruction("rrrr,(nnn,rr)", Instructions._rrrr_Innn_rrI);              // OK
            PushInstruction("rrrr,(nnn,rrr)", Instructions._rrrr_Innn_rrrI);        // Duplicate 4      // OK
            PushInstruction("rrrr,(rrr)", Instructions._rrrr_IrrrI);            // OK
            PushInstruction("rrrr,(rrr,nnn)", Instructions._rrrr_Irrr_nnnI);        // Duplicate 4      // OK
            PushInstruction("rrrr,(rrr,r)", Instructions._rrrr_Irrr_rI);        // OK
            PushInstruction("rrrr,(rrr,rr)", Instructions._rrrr_Irrr_rrI);      // OK
            PushInstruction("rrrr,(rrr,rrr)", Instructions._rrrr_Irrr_rrrI);    // OK
            PushInstruction("rrrr,fr", Instructions._rrrr_fr);      //          // OK

            PushInstruction("(nnn),nnnn,n", Instructions._InnnI_nnnn_n);             // OK
            PushInstruction("(nnn),nnnn,n,nnn", Instructions._InnnI_nnnn_n_nnn);     // OK
            PushInstruction("(nnn),r", Instructions._InnnI_r);                      // OK
            PushInstruction("(nnn),rr", Instructions._InnnI_rr);                    // OK
            PushInstruction("(nnn),rrr", Instructions._InnnI_rrr);                  // OK
            PushInstruction("(nnn),rrrr", Instructions._InnnI_rrrr);                // OK
            PushInstruction("(nnn),(nnn),n,rrr", Instructions._InnnI_InnnI_n_rrr);              // OK
            PushInstruction("(nnn),(nnn,nnn),n,rrr", Instructions._InnnI_Innn_nnnI_n_rrr);      // OK
            PushInstruction("(nnn),(nnn,r),n,rrr", Instructions._InnnI_Innn_rI_n_rrr);          // OK
            PushInstruction("(nnn),(nnn,rr),n,rrr", Instructions._InnnI_Innn_rrI_n_rrr);        // OK
            PushInstruction("(nnn),(nnn,rrr),n,rrr", Instructions._InnnI_Innn_rrrI_n_rrr);        // Duplicate 5        // OK
            PushInstruction("(nnn),(rrr),n,rrr", Instructions._InnnI_IrrrI_n_rrr);              // OK
            PushInstruction("(nnn),(rrr,nnn),n,rrr", Instructions._InnnI_Irrr_nnnI_n_rrr);        // Duplicate 5        // OK
            PushInstruction("(nnn),(rrr,r),n,rrr", Instructions._InnnI_Irrr_rI_n_rrr);          // OK
            PushInstruction("(nnn),(rrr,rr),n,rrr", Instructions._InnnI_Irrr_rrI_n_rrr);        // OK
            PushInstruction("(nnn),(rrr,rrr),n,rrr", Instructions._InnnI_Irrr_rrrI_n_rrr);      // OK
            PushInstruction("(nnn),fr", Instructions._InnnI_fr);     //                         // OK

            PushInstruction("(nnn,nnn),nnnn,n", Instructions._Innn_nnnI_nnnn_n);                 // OK
            PushInstruction("(nnn,nnn),nnnn,n,nnn", Instructions._Innn_nnnI_nnnn_n_nnn);         // OK
            PushInstruction("(nnn,nnn),r", Instructions._Innn_nnnI_r);                          // OK
            PushInstruction("(nnn,nnn),rr", Instructions._Innn_nnnI_rr);                        // OK
            PushInstruction("(nnn,nnn),rrr", Instructions._Innn_nnnI_rrr);                      // OK
            PushInstruction("(nnn,nnn),rrrr", Instructions._Innn_nnnI_rrrr);                    // OK
            PushInstruction("(nnn,nnn),(nnn),n,rrr", Instructions._Innn_nnnI_InnnI_n_rrr);              // OK
            PushInstruction("(nnn,nnn),(nnn,nnn),n,rrr", Instructions._Innn_nnnI_Innn_nnnI_n_rrr);      // OK
            PushInstruction("(nnn,nnn),(nnn,r),n,rrr", Instructions._Innn_nnnI_Innn_rI_n_rrr);          // OK
            PushInstruction("(nnn,nnn),(nnn,rr),n,rrr", Instructions._Innn_nnnI_Innn_rrI_n_rrr);            // OK
            PushInstruction("(nnn,nnn),(nnn,rrr),n,rrr", Instructions._Innn_nnnI_Innn_rrrI_n_rrr);        // Duplicate 6            // OK
            PushInstruction("(nnn,nnn),(rrr),n,rrr", Instructions._Innn_nnnI_IrrrI_n_rrr);                      // OK
            PushInstruction("(nnn,nnn),(rrr,nnn),n,rrr", Instructions._Innn_nnnI_Irrr_nnnI_n_rrr);        // Duplicate 6            // OK
            PushInstruction("(nnn,nnn),(rrr,r),n,rrr", Instructions._Innn_nnnI_Irrr_rI_n_rrr);          // OK
            PushInstruction("(nnn,nnn),(rrr,rr),n,rrr", Instructions._Innn_nnnI_Irrr_rrI_n_rrr);        // OK
            PushInstruction("(nnn,nnn),(rrr,rrr),n,rrr", Instructions._Innn_nnnI_Irrr_rrrI_n_rrr);      // OK
            PushInstruction("(nnn,nnn),fr", Instructions._Innn_nnnI_fr);     //                         // OK

            PushInstruction("(nnn,r),nnnn,n", Instructions._Innn_rI_nnnn_n);                 // OK
            PushInstruction("(nnn,r),nnnn,n,nnn", Instructions._Innn_rI_nnnn_n_nnn);         // OK
            PushInstruction("(nnn,r),r", Instructions._Innn_rI_r);                          // OK
            PushInstruction("(nnn,r),rr", Instructions._Innn_rI_rr);                        // OK
            PushInstruction("(nnn,r),rrr", Instructions._Innn_rI_rrr);                      // OK
            PushInstruction("(nnn,r),rrrr", Instructions._Innn_rI_rrrr);                    // OK
            PushInstruction("(nnn,r),(nnn),n,rrr", Instructions._Innn_rI_InnnI_n_rrr);            // OK
            PushInstruction("(nnn,r),(nnn,nnn),n,rrr", Instructions._Innn_rI_Innn_nnnI_n_rrr);      // OK
            PushInstruction("(nnn,r),(nnn,r),n,rrr", Instructions._Innn_rI_Innn_rI_n_rrr);          // OK
            PushInstruction("(nnn,r),(nnn,rr),n,rrr", Instructions._Innn_rI_Innn_rrI_n_rrr);        // OK
            PushInstruction("(nnn,r),(nnn,rrr),n,rrr", Instructions._Innn_rI_Innn_rrrI_n_rrr);            // Duplicate 7        // OK 
            PushInstruction("(nnn,r),(rrr),n,rrr", Instructions._Innn_rI_IrrrI_n_rrr);              // OK
            PushInstruction("(nnn,r),(rrr,nnn),n,rrr", Instructions._Innn_rI_Irrr_nnnI_n_rrr);            // Duplicate 7        // OK
            PushInstruction("(nnn,r),(rrr,r),n,rrr", Instructions._Innn_rI_Irrr_rI_n_rrr);          // OK
            PushInstruction("(nnn,r),(rrr,rr),n,rrr", Instructions._Innn_rI_Irrr_rrI_n_rrr);        // OK
            PushInstruction("(nnn,r),(rrr,rrr),n,rrr", Instructions._Innn_rI_Irrr_rrrI_n_rrr);      // OK
            PushInstruction("(nnn,r),fr", Instructions._Innn_rI_fr);       //                       // OK

            PushInstruction("(nnn,rr),nnnn,n", Instructions._Innn_rrI_nnnn_n);                       // OK
            PushInstruction("(nnn,rr),nnnn,n,nnn", Instructions._Innn_rrI_nnnn_n_nnn);               // OK
            PushInstruction("(nnn,rr),r", Instructions._Innn_rrI_r);                                // OK
            PushInstruction("(nnn,rr),rr", Instructions._Innn_rrI_rr);                              // OK
            PushInstruction("(nnn,rr),rrr", Instructions._Innn_rrI_rrr);                            // OK
            PushInstruction("(nnn,rr),rrrr", Instructions._Innn_rrI_rrrr);                          // OK
            PushInstruction("(nnn,rr),(nnn),n,rrr", Instructions._Innn_rrI_InnnI_n_rrr);                  // OK
            PushInstruction("(nnn,rr),(nnn,nnn),n,rrr", Instructions._Innn_rrI_Innn_nnnI_n_rrr);          // OK
            PushInstruction("(nnn,rr),(nnn,r),n,rrr", Instructions._Innn_rrI_Innn_rI_n_rrr);              // OK
            PushInstruction("(nnn,rr),(nnn,rr),n,rrr", Instructions._Innn_rrI_Innn_rrI_n_rrr);            // OK
            PushInstruction("(nnn,rr),(nnn,rrr),n,rrr", Instructions._Innn_rrI_Innn_rrrI_n_rrr);          // Duplicate 8          // OK
            PushInstruction("(nnn,rr),(rrr),n,rrr", Instructions._Innn_rrI_IrrrI_n_rrr);                  // OK
            PushInstruction("(nnn,rr),(rrr,nnn),n,rrr", Instructions._Innn_rrI_Irrr_nnnI_n_rrr);          // Duplicate 8          // OK
            PushInstruction("(nnn,rr),(rrr,r),n,rrr", Instructions._Innn_rrI_Irrr_rI_n_rrr);              // OK
            PushInstruction("(nnn,rr),(rrr,rr),n,rrr", Instructions._Innn_rrI_Irrr_rrI_n_rrr);            // OK
            PushInstruction("(nnn,rr),(rrr,rrr),n,rrr", Instructions._Innn_rrI_Irrr_rrrI_n_rrr);          // OK
            PushInstruction("(nnn,rr),fr", Instructions._Innn_rrI_fr);      //                  // OK

            PushInstruction("(nnn,rrr),nnnn,n", Instructions._Innn_rrrI_nnnn_n);                 // Duplicate 11             // OK
            PushInstruction("(nnn,rrr),nnnn,n,nnn", Instructions._Innn_rrrI_nnnn_n_nnn);         // Duplicate 12             // OK
            PushInstruction("(nnn,rrr),r", Instructions._Innn_rrrI_r);                          // Duplicate 13             // OK
            PushInstruction("(nnn,rrr),rr", Instructions._Innn_rrrI_rr);                        // Duplicate 14             // OK
            PushInstruction("(nnn,rrr),rrr", Instructions._Innn_rrrI_rrr);                      // Duplicate 15             // OK
            PushInstruction("(nnn,rrr),rrrr", Instructions._Innn_rrrI_rrrr);                    // Duplicate 16             // OK
            PushInstruction("(nnn,rrr),(nnn),n,rrr", Instructions._Innn_rrrI_InnnI_n_rrr);            // Duplicate 17       // OK
            PushInstruction("(nnn,rrr),(nnn,nnn),n,rrr", Instructions._Innn_rrrI_Innn_nnnI_n_rrr);    // Duplicate 18       // OK
            PushInstruction("(nnn,rrr),(nnn,r),n,rrr", Instructions._Innn_rrrI_Innn_rI_n_rrr);        // Duplicate 19       // OK
            PushInstruction("(nnn,rrr),(nnn,rr),n,rrr", Instructions._Innn_rrrI_Innn_rrI_n_rrr);      // Duplicate 20       // OK
            PushInstruction("(nnn,rrr),(nnn,rrr),n,rrr", Instructions._Innn_rrrI_Innn_rrrI_n_rrr);    // Duplicate 21       // OK
            PushInstruction("(nnn,rrr),(rrr),n,rrr", Instructions._Innn_rrrI_IrrrI_n_rrr);            // Duplicate 22       // OK
            PushInstruction("(nnn,rrr),(rrr,nnn),n,rrr", Instructions._Innn_rrrI_Irrr_nnnI_n_rrr);    // Duplicate 23       // OK
            PushInstruction("(nnn,rrr),(rrr,r),n,rrr", Instructions._Innn_rrrI_Irrr_rI_n_rrr);        // Duplicate 24       // OK
            PushInstruction("(nnn,rrr),(rrr,rr),n,rrr", Instructions._Innn_rrrI_Irrr_rrI_n_rrr);      // Duplicate 25       // OK
            PushInstruction("(nnn,rrr),(rrr,rrr),n,rrr", Instructions._Innn_rrrI_Irrr_rrrI_n_rrr);    // Duplicate 26       // OK
            PushInstruction("(nnn,rrr),fr", Instructions._Innn_rrrI_fr);     //                 // Duplicate 9              // OK

            PushInstruction("(rrr),nnnn,n", Instructions._IrrrI_nnnn_n);                            // OK
            PushInstruction("(rrr),nnnn,n,nnn", Instructions._IrrrI_nnnn_n_nnn);                    // OK
            PushInstruction("(rrr),r", Instructions._IrrrI_r);                                      // OK
            PushInstruction("(rrr),rr", Instructions._IrrrI_rr);                                    // OK
            PushInstruction("(rrr),rrr", Instructions._IrrrI_rrr);                                  // OK
            PushInstruction("(rrr),rrrr", Instructions._IrrrI_rrrr);                                // OK
            PushInstruction("(rrr),(nnn),n,rrr", Instructions._IrrrI_InnnI_n_rrr);                  // OK
            PushInstruction("(rrr),(nnn,nnn),n,rrr", Instructions._IrrrI_Innn_nnnI_n_rrr);          // OK
            PushInstruction("(rrr),(nnn,r),n,rrr", Instructions._IrrrI_Innn_rI_n_rrr);              // OK
            PushInstruction("(rrr),(nnn,rr),n,rrr", Instructions._IrrrI_Innn_rrI_n_rrr);            // OK
            PushInstruction("(rrr),(nnn,rrr),n,rrr", Instructions._IrrrI_Innn_rrrI_n_rrr);                // Duplicate 10       // OK
            PushInstruction("(rrr),(rrr),n,rrr", Instructions._IrrrI_IrrrI_n_rrr);                  // OK
            PushInstruction("(rrr),(rrr,nnn),n,rrr", Instructions._IrrrI_Irrr_nnnI_n_rrr);                // Duplicate 10       // OK
            PushInstruction("(rrr),(rrr,r),n,rrr", Instructions._IrrrI_Irrr_rI_n_rrr);              // OK
            PushInstruction("(rrr),(rrr,rr),n,rrr", Instructions._IrrrI_Irrr_rrI_n_rrr);            // OK
            PushInstruction("(rrr),(rrr,rrr),n,rrr", Instructions._IrrrI_Irrr_rrrI_n_rrr);          // OK
            PushInstruction("(rrr),fr", Instructions._IrrrI_fr);         //

            PushInstruction("(rrr,nnn),nnnn,n", Instructions._Irrr_nnnI_nnnn_n);                     // Duplicate 11
            PushInstruction("(rrr,nnn),nnnn,n,nnn", Instructions._Irrr_nnnI_nnnn_n_nnn);             // Duplicate 12
            PushInstruction("(rrr,nnn),r", Instructions._Irrr_nnnI_r);                              // Duplicate 13
            PushInstruction("(rrr,nnn),rr", Instructions._Irrr_nnnI_rr);                            // Duplicate 14
            PushInstruction("(rrr,nnn),rrr", Instructions._Irrr_nnnI_rrr);                          // Duplicate 15
            PushInstruction("(rrr,nnn),rrrr", Instructions._Irrr_nnnI_rrrr);                        // Duplicate 16
            PushInstruction("(rrr,nnn),(nnn),n,rrr", Instructions._Irrr_nnnI_InnnI_n_rrr);                // Duplicate 17
            PushInstruction("(rrr,nnn),(nnn,nnn),n,rrr", Instructions._Irrr_nnnI_Innn_nnnI_n_rrr);        // Duplicate 18
            PushInstruction("(rrr,nnn),(nnn,r),n,rrr", Instructions._Irrr_nnnI_Innn_rI_n_rrr);            // Duplicate 19
            PushInstruction("(rrr,nnn),(nnn,rr),n,rrr", Instructions._Irrr_nnnI_Innn_rrI_n_rrr);          // Duplicate 20
            PushInstruction("(rrr,nnn),(nnn,rrr),n,rrr", Instructions._Irrr_nnnI_Innn_rrrI_n_rrr);        // Duplicate 21     // Duplicate 27
            PushInstruction("(rrr,nnn),(rrr),n,rrr", Instructions._Irrr_nnnI_IrrrI_n_rrr);                // Duplicate 22
            PushInstruction("(rrr,nnn),(rrr,nnn),n,rrr", Instructions._Irrr_nnnI_Irrr_nnnI_n_rrr);        // Duplicate 23     // Duplicate 27
            PushInstruction("(rrr,nnn),(rrr,r),n,rrr", Instructions._Irrr_nnnI_Irrr_rI_n_rrr);            // Duplicate 24
            PushInstruction("(rrr,nnn),(rrr,rr),n,rrr", Instructions._Irrr_nnnI_Irrr_rrI_n_rrr);          // Duplicate 25
            PushInstruction("(rrr,nnn),(rrr,rrr),n,rrr", Instructions._Irrr_nnnI_Irrr_rrrI_n_rrr);        // Duplicate 26
            PushInstruction("(rrr,nnn),fr", Instructions._Irrr_nnnI_fr);         //                 // Duplicate 9

            PushInstruction("(rrr,r),nnnn,n", Instructions._Irrr_rI_nnnn_n);
            PushInstruction("(rrr,r),nnnn,n,nnn", Instructions._Irrr_rI_nnnn_n_nnn);
            PushInstruction("(rrr,r),r", Instructions._Irrr_rI_r);
            PushInstruction("(rrr,r),rr", Instructions._Irrr_rI_rr);
            PushInstruction("(rrr,r),rrr", Instructions._Irrr_rI_rrr);
            PushInstruction("(rrr,r),rrrr", Instructions._Irrr_rI_rrrr);
            PushInstruction("(rrr,r),(nnn),n,rrr", Instructions._Irrr_rI_InnnI_n_rrr);
            PushInstruction("(rrr,r),(nnn,nnn),n,rrr", Instructions._Irrr_rI_Innn_nnnI_n_rrr);
            PushInstruction("(rrr,r),(nnn,r),n,rrr", Instructions._Irrr_rI_Innn_rI_n_rrr);
            PushInstruction("(rrr,r),(nnn,rr),n,rrr", Instructions._Irrr_rI_Innn_rrI_n_rrr);
            PushInstruction("(rrr,r),(nnn,rrr),n,rrr", Instructions._Irrr_rI_Innn_rrrI_n_rrr);        // Duplicate 28
            PushInstruction("(rrr,r),(rrr),n,rrr", Instructions._Irrr_rI_IrrrI_n_rrr);
            PushInstruction("(rrr,r),(rrr,nnn),n,rrr", Instructions._Irrr_rI_Irrr_nnnI_n_rrr);        // Duplicate 28
            PushInstruction("(rrr,r),(rrr,r),n,rrr", Instructions._Irrr_rI_Irrr_rI_n_rrr);
            PushInstruction("(rrr,r),(rrr,rr),n,rrr", Instructions._Irrr_rI_Irrr_rrI_n_rrr);
            PushInstruction("(rrr,r),(rrr,rrr),n,rrr", Instructions._Irrr_rI_Irrr_rrrI_n_rrr);
            PushInstruction("(rrr,r),fr", Instructions._Irrr_rI_fr);           //

            PushInstruction("(rrr,rr),nnnn,n", Instructions._Irrr_rrI_nnnn_n);
            PushInstruction("(rrr,rr),nnnn,n,nnn", Instructions._Irrr_rrI_nnnn_n_nnn);
            PushInstruction("(rrr,rr),r", Instructions._Irrr_rrI_r);
            PushInstruction("(rrr,rr),rr", Instructions._Irrr_rrI_rr);
            PushInstruction("(rrr,rr),rrr", Instructions._Irrr_rrI_rrr);
            PushInstruction("(rrr,rr),rrrr", Instructions._Irrr_rrI_rrrr);
            PushInstruction("(rrr,rr),(nnn),n,rrr", Instructions._Irrr_rrI_InnnI_n_rrr);
            PushInstruction("(rrr,rr),(nnn,nnn),n,rrr", Instructions._Irrr_rrI_Innn_nnnI_n_rrr);
            PushInstruction("(rrr,rr),(nnn,r),n,rrr", Instructions._Irrr_rrI_Innn_rI_n_rrr);
            PushInstruction("(rrr,rr),(nnn,rr),n,rrr", Instructions._Irrr_rrI_Innn_rrI_n_rrr);
            PushInstruction("(rrr,rr),(nnn,rrr),n,rrr", Instructions._Irrr_rrI_Innn_rrrI_n_rrr);          // Duplicate 29
            PushInstruction("(rrr,rr),(rrr),n,rrr", Instructions._Irrr_rrI_IrrrI_n_rrr);
            PushInstruction("(rrr,rr),(rrr,nnn),n,rrr", Instructions._Irrr_rrI_Irrr_nnnI_n_rrr);          // Duplicate 29
            PushInstruction("(rrr,rr),(rrr,r),n,rrr", Instructions._Irrr_rrI_Irrr_rI_n_rrr);
            PushInstruction("(rrr,rr),(rrr,rr),n,rrr", Instructions._Irrr_rrI_Irrr_rrI_n_rrr);
            PushInstruction("(rrr,rr),(rrr,rrr),n,rrr", Instructions._Irrr_rrI_Irrr_rrrI_n_rrr);
            PushInstruction("(rrr,rr),fr", Instructions._Irrr_rrI_fr);          //

            PushInstruction("(rrr,rrr),nnnn,n", Instructions._Irrr_rrrI_nnnn_n);
            PushInstruction("(rrr,rrr),nnnn,n,nnn", Instructions._Irrr_rrrI_nnnn_n_nnn);
            PushInstruction("(rrr,rrr),r", Instructions._Irrr_rrrI_r);
            PushInstruction("(rrr,rrr),rr", Instructions._Irrr_rrrI_rr);
            PushInstruction("(rrr,rrr),rrr", Instructions._Irrr_rrrI_rrr);
            PushInstruction("(rrr,rrr),rrrr", Instructions._Irrr_rrrI_rrrr);
            PushInstruction("(rrr,rrr),(nnn),n,rrr", Instructions._Irrr_rrrI_InnnI_n_rrr);
            PushInstruction("(rrr,rrr),(nnn,nnn),n,rrr", Instructions._Irrr_rrrI_Innn_nnnI_n_rrr);
            PushInstruction("(rrr,rrr),(nnn,r),n,rrr", Instructions._Irrr_rrrI_Innn_rI_n_rrr);
            PushInstruction("(rrr,rrr),(nnn,rr),n,rrr", Instructions._Irrr_rrrI_Innn_rrI_n_rrr);
            PushInstruction("(rrr,rrr),(nnn,rrr),n,rrr", Instructions._Irrr_rrrI_Innn_rrrI_n_rrr);        // Duplicate 30
            PushInstruction("(rrr,rrr),(rrr),n,rrr", Instructions._Irrr_rrrI_IrrrI_n_rrr);
            PushInstruction("(rrr,rrr),(rrr,nnn),n,rrr", Instructions._Irrr_rrrI_Irrr_nnnI_n_rrr);        // Duplicate 30
            PushInstruction("(rrr,rrr),(rrr,r),n,rrr", Instructions._Irrr_rrrI_Irrr_rI_n_rrr);
            PushInstruction("(rrr,rrr),(rrr,rr),n,rrr", Instructions._Irrr_rrrI_Irrr_rrI_n_rrr);
            PushInstruction("(rrr,rrr),(rrr,rrr),n,rrr", Instructions._Irrr_rrrI_Irrr_rrrI_n_rrr);
            PushInstruction("(rrr,rrr),fr", Instructions._Irrr_rrrI_fr);         //

            PushInstruction("fr,nnnn", Instructions._fr_nnnn);               // OK
            PushInstruction("fr,r", Instructions._fr_r);                    // OK
            PushInstruction("fr,rr", Instructions._fr_rr);                  // OK      
            PushInstruction("fr,rrr", Instructions._fr_rrr);                // OK
            PushInstruction("fr,rrrr", Instructions._fr_rrrr);              // OK
            PushInstruction("fr,(nnn)", Instructions._fr_InnnI);            // OK
            PushInstruction("fr,(nnn,nnn)", Instructions._fr_Innn_nnnI);    // OK
            PushInstruction("fr,(nnn,r)", Instructions._fr_Innn_rI);        // OK
            PushInstruction("fr,(nnn,rr)", Instructions._fr_Innn_rrI);      // OK
            PushInstruction("fr,(nnn,rrr)", Instructions._fr_Innn_rrrI);            // Duplicate 31     // OK
            PushInstruction("fr,(rrr)", Instructions._fr_IrrrI);            // OK
            PushInstruction("fr,(rrr,nnn)", Instructions._fr_Irrr_nnnI);            // Duplicate 31     // OK
            PushInstruction("fr,(rrr,r)", Instructions._fr_Irrr_rI);        // OK
            PushInstruction("fr,(rrr,rr)", Instructions._fr_Irrr_rrI);      // OK
            PushInstruction("fr,(rrr,rrr)", Instructions._fr_Irrr_rrrI);        // OK
            PushInstruction("fr,fr", Instructions._fr_fr);                //        // OK
        }
        
        private static void InitializeOpCodeDefinition()
        {
            //opCodeDefinition.Add("NOP", 0x00);
            opCodeDefinition.Add("LD", 0x01);
            opCodeDefinition.Add("ADD", 0x02);
            opCodeDefinition.Add("SUB", 0x03);
            // more opcodes as needed
        }

        private static void PushInstruction(string subInstruction, byte subOp)
        {
            //if (subInstruction == "(nnn),nnnn,n")
            //{
            //    int x = 0;
            //}

            string instructionBits = GetInstructionBitFormat(currentInstructionString, subInstruction.Split(','));

            tempReport += $"{currentInstructionString} {subInstruction}; {instructionBits}\n";

            Mnem.AddSubOp($"{currentInstructionString} {subInstruction}", currentInstruction, instructionBits, subOp);
            
        }

        private static string GetInstructionBitFormat(string instruction, string[] args)
        {
            List<string> bitArgs = [];
            byte argIndex = 0;

            byte instructionBits =
                instruction.EndsWith("16", StringComparison.Ordinal) ? (byte)16 :
                instruction.EndsWith("24", StringComparison.Ordinal) ? (byte)24 :
                instruction.EndsWith("32", StringComparison.Ordinal) ? (byte)32 :
                (byte)8;

            string rawInstruction = StripSizeSuffix(instruction).ToUpper();

            bitArgs.Add("oooooooo");

            byte argumentOrdinal = 0;

            foreach (string arg in args)
            {
                if (arg.StartsWith("r") || arg.StartsWith("(r"))    // current format: uuuNNNNN
                {
                    bitArgs.Add("uuu" + new string((char)('A' + argIndex), 5));
                    argIndex++;
                }
                else if (arg == "nnn)")
                {
                    bitArgs.Add(new string((char)('A' + argIndex), 8));
                    bitArgs.Add(new string((char)('A' + argIndex), 8));
                    bitArgs.Add(new string((char)('A' + argIndex), 8));
                    argIndex++;
                }
                else if (arg.StartsWith("n"))
                {
                    if (argumentOrdinal > 0)
                    {
                        string token = args[argumentOrdinal - 1];
                        int repeat = token.Length;

                        if (repeat > 0 && token.All(c => c == 'r'))
                        {
                            string pattern = new((char)('A' + argIndex), 8);
                            for (int i = 0; i < repeat; i++)
                                bitArgs.Add(pattern);
                        } else if (token == "(nnn)" || token == "(rrr)")
                        {
                            for (int i = 0; i < arg.Length; i++)
                            {
                                bitArgs.Add(new string((char)('A' + argIndex), 8));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < arg.Length; i++)
                            {
                                bitArgs.Add(new string((char)('A' + argIndex), 8));
                            }
                        }
                    } else
                    {
                        for (int i = 0; i < arg.Length; i++)
                        {
                            bitArgs.Add(new string((char)('A' + argIndex), 8));
                        }
                    }
                    
                    argIndex++;
                }
                else if (arg.StartsWith("(n"))
                {
                    bitArgs.Add(new string((char)('A' + argIndex), 8));
                    bitArgs.Add(new string((char)('A' + argIndex), 8));
                    bitArgs.Add(new string((char)('A' + argIndex), 8));
                    argIndex++;
                }
                else if (arg.Equals("fr"))
                {
                    bitArgs.Add("uuuu" + new string((char)('A' + argIndex), 4));
                    argIndex++;
                }

                argumentOrdinal++;
            }

            return string.Join(" ", bitArgs.ToArray());
        }

        private static string StripSizeSuffix(string instr)
        {
            ReadOnlySpan<char> s = instr;
            int len = s.Length;

            if (len >= 2)
            {
                char c1 = s[len - 2];
                char c2 = s[len - 1];

                // check for “16”, “24” or “32”
                if ((c1 == '1' && c2 == '6') ||
                    (c1 == '2' && c2 == '4') ||
                    (c1 == '3' && c2 == '2'))
                {
                    // slice off the last two chars and reify to string
                    return s[..(len - 2)].ToString();
                }
            }

            // no matching suffix → return original
            return instr;
        }
    }
}
