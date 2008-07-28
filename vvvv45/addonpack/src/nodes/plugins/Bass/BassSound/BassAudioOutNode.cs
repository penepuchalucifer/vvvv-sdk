using System;
using System.Collections.Generic;
using System.Text;
using VVVV.PluginInterfaces.V1;
using Un4seen.Bass;

namespace BassSound
{
    public class BassAudioOutNode : IPlugin
    {
        #region Plugin Information
        public static IPluginInfo PluginInfo
        {
            get
            {
                IPluginInfo Info = new PluginInfo();
                Info.Name = "AudioOut";
                Info.Category = "Bass";
                Info.Version = "";
                Info.Help = "Audio out only for WDM handles";
                Info.Bugs = "";
                Info.Credits = "";
                Info.Warnings = "";

                //leave below as is
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
                System.Diagnostics.StackFrame sf = st.GetFrame(0);
                System.Reflection.MethodBase method = sf.GetMethod();
                Info.Namespace = method.DeclaringType.Namespace;
                Info.Class = method.DeclaringType.Name;
                return Info;
            }
        }
        #endregion

        protected IPluginHost FHost;
        private List<int> FChannels = new List<int>();

        private IValueIn FPinInHandle;

        private IValueIn FPInInPan;
        private IValueIn FPinInVolume;

        public void SetPluginHost(IPluginHost Host)
        {
            this.FHost = Host;

            this.FHost.CreateValueInput("HandleIn", 1, null, TSliceMode.Single, TPinVisibility.True, out this.FPinInHandle);
            this.FPinInHandle.SetSubType(double.MinValue, double.MaxValue,1, 0, false, false, true);

            //this.FHost.CreateValueInput("Pan", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out this.FPInInPan);
            //this.FPInInPan.SetSubType(-1, 1, 0.01, 0, false, false, false);

            //this.FHost.CreateValueInput("Volume", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out this.FPinInVolume);
            //this.FPinInVolume.SetSubType(0, 1, 0.01, 0, false, false, false);

        }

        public void Configurate(IPluginConfig Input)
        {

        }

        public void Evaluate(int SpreadMax)
        {
            //Do nothing for the moment, can still use as a feed for file stream
            /*
            if (this.FPinInHandle.IsConnected && this.FPinInHandle.PinIsChanged)
            {
                StopChannels();
                this.FChannels.Clear();
                //Browse channel list
                for (int i = 0; i < this.FPinInHandle.SliceCount; i++)
                {
                    double dhandle;
                    this.FPinInHandle.GetValue(0,out dhandle);
                    int handle = Convert.ToInt32(dhandle);
                    this.FChannels.Add(handle);
                    Bass.BASS_ChannelPlay(handle, false);
                }
            }
            else
            {
                StopChannels();
            }*/
        }


        public bool AutoEvaluate
        {
            get { return true; }
        }

        public void Dispose()
        {

        }
    }
}
