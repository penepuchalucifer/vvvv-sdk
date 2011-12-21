﻿#region usings
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Collections.Generic;

using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
using Phidgets;

#endregion usings

namespace VVVV.Nodes
{
    #region PluginInfo
    [PluginInfo(Name = "IO",
                Category = "Device",
                Version = "Phidget",
                Help = "Wrapper for the Phidget IOBoards",
                Tags = "Controller,IO,InterfaceKit",
                Author = "Phlegma",
                AutoEvaluate = true
)]
    #endregion
    public class InterfaceBoard : IPluginEvaluate
    {
        #region fields & pins

        //Input 
        [Input("Digital", DefaultValue = 0)]
        IDiffSpread<bool> FDigitalIn;

        [Input("Radiometric", IsSingle = true)]
        IDiffSpread<bool> FRadiometricIn;

        [Input("DataRate (ms)", DefaultValue = 16, Visibility = PinVisibility.OnlyInspector)]
        IDiffSpread<int> FDataRateIn;

        [Input("Serial", DefaultValue = 0, IsSingle = true, AsInt = true, MinValue = 0, MaxValue = int.MaxValue)]
        IDiffSpread<int> FSerial;



        //Output
        [Output("Attached")]
        ISpread<bool> FAttached;

        [Output("Sensor")]
        ISpread<double> FSensorOut;

        [Output("Digital")]
        ISpread<bool> FDigitalOut;

        [Output("Radiometric", Visibility = PinVisibility.OnlyInspector)]
        ISpread<bool> FRadiometricOut;

        [Output("DataRate(ms)", Visibility = PinVisibility.OnlyInspector)]
        ISpread<int> FDataRateOut;

        [Output("DataRateMin(ms)", Visibility = PinVisibility.OnlyInspector)]
        ISpread<int> FDataRateMinOut;

        [Output("DataRateMax(ms)", Visibility = PinVisibility.OnlyInspector)]
        ISpread<int> FDataRateMaxOut;





        //Logger
        [Import()]
        ILogger FLogger;


        //private Fields
        WrapperIOBoards FIO;
        private bool disposed;
        private bool FInit = true;

        #endregion fields & pins

        //called when data for any output pin is requested
        public void Evaluate(int SpreadMax)
        {
            try
            {
                if (FSerial.IsChanged)
                {
                    if (FIO != null)
                    {
                        FIO = null;
                    }
                    FIO = new WrapperIOBoards(FSerial[0]);
                    FInit = true;
                }

                if (FIO.Attached)
                {

                    if (FInit)
                    {
                        FDigitalOut.SliceCount = FIO.GetInputCount();
                        FSensorOut.SliceCount = FIO.GetSensorCount();
                        FDataRateMaxOut.SliceCount = FIO.GetSensorCount();
                        FDataRateMinOut.SliceCount = FIO.GetSensorCount();
                        FDataRateOut.SliceCount = FIO.GetSensorCount();
                    }

                    if (!(FIO.FPhidget.ID == Phidget.PhidgetID.LINEAR_TOUCH || FIO.FPhidget.ID == Phidget.PhidgetID.ROTARY_TOUCH))
                    {
                        if (FRadiometricIn.IsChanged || FInit)
                            FIO.SetRadiometric(FRadiometricIn[0]);

                        if (FDataRateIn.IsChanged || FInit)
                        {
                            for (int i = 0; i < FIO.GetSensorCount(); i++)
                                FIO.SetDataRate(i, FDataRateIn[i]);
                        }
                    }

                    if (FIO.Changed || FInit)
                    {
                        for (int i = 0; i < FIO.GetInputCount(); i++)
                            FDigitalOut[i] = FIO.GetInputState(i);
                    }

                    for (int i = 0; i < FIO.GetSensorCount(); i++)
                    {
                        if (!(FIO.FPhidget.ID == Phidget.PhidgetID.LINEAR_TOUCH || FIO.FPhidget.ID == Phidget.PhidgetID.ROTARY_TOUCH))
                        {
                            FSensorOut[i] = Convert.ToDouble(FIO.GetSensorRawValue(i)) / 4095;
                            FDataRateOut[i] = FIO.GetDataRate(i);
                            FDataRateMinOut[i] = FIO.GetDataRateMin(i);
                            FDataRateMaxOut[i] = FIO.GetDataRateMax(i);
                        }
                        else
                        {
                            FSensorOut[i] = Convert.ToDouble(FIO.GetSensorValue(i)) / 1000;
                            FDataRateOut[i] = FIO.GetDataRate(i);
                            FDataRateMinOut[i] = FIO.GetDataRateMin(i);
                            FDataRateMaxOut[i] = FIO.GetDataRateMax(i);
                        }

                    }


                    if (FDigitalIn.IsChanged || FInit)
                    {
                        for (int i = 0; i < SpreadMax; i++)
                        {
                            if (i < FIO.Count)
                            {
                                if (FDigitalIn.IsChanged)
                                    FIO.SetDigitalOutput(i, FDigitalIn[i]);
                            }
                        }
                    }



                    FInit = false;
                }

                FAttached[0] = FIO.Attached;

                List<PhidgetException> Exceptions = FIO.Errors;
                if (Exceptions != null)
                {
                    foreach (Exception e in Exceptions)
                        FLogger.Log(e);
                }
            }
            catch (PhidgetException ex)
            {
                FLogger.Log(ex);
            }
        }
    }
}
