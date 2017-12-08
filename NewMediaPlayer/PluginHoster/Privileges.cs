using LunalipseAPI.Configuration;
using LunalipseAPI.Generic;
using LunalipseAPI.PlayMode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NewMediaPlayer.PluginHoster
{
    partial class PluginHelper
    {
        ArrayList ConfigIOPrivilege = new ArrayList();
        ArrayList DrawingUIPrivilege = new ArrayList();
        ArrayList CustomModePrivilege = new ArrayList();
        ArrayList LunalipxExtendPrivilege = new ArrayList();
        ArrayList NetworkingPrivilege = new ArrayList();
        ArrayList SubProgreame = new ArrayList();
        IDictionary<string, ArrayList> reqPriv = new Dictionary<string, ArrayList>();

        ArrayList _pal = new ArrayList();
        private void GetPluginPrivilege(Type _T)
        {
            if(_T.GetCustomAttribute(typeof(LunalipseDrawing)) != null)
            {
                _pal.Add("uidraw");
            }
            else if(_T.GetCustomAttribute(typeof(GlobalConfigPrivilege)) != null)
            {
                _pal.Add("gcfgpv");
            }
            else if(_T.GetCustomAttribute(typeof(LunalipseCustomMode)) != null)
            {
                _pal.Add("cmode");
            }
            else if(_T.GetCustomAttribute(typeof(LunalipxExtend)) != null)
            {
                _pal.Add("lpxE");
            }
        }

        private void ApplyPrevilege(string _pn)
        {
            if(reqPriv.ContainsKey(_pn))
            {
                _pal.Clear();
                return;
            }
            reqPriv.Add(_pn, _pal.Clone() as ArrayList);
            _pal.Clear();
        }

        public ArrayList GetPrivilegeByName(string name)
        {
            return reqPriv[name];
        }

        public void RemovePrivilegeFrom(string name)
        {
            reqPriv.Remove(name);
        }
    }
}
