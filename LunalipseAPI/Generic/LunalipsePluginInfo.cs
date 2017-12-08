using System;

namespace LunalipseAPI.Generic
{
    public class LunalipsePluginInfo: Attribute
    {
        public string Author, Description, Team, Name;
        public string Version;
        public bool autoLoad;
        public LunalipsePluginInfo()
        {
            Author = "Anonymous";
            Description = "This is a plugin";
            Team = "";
            Name = "LXP Plugin";
            Version = "1.0.0.0";
            autoLoad = false;
        }
    }
}
