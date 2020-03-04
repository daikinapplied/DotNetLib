using System.Collections.Generic;

namespace Daikin.DotNetLib.MsTeams.Models
{
    public class Section
    {
        #region Propeties
        public List<Fact> Facts { get; set; }

        public string Text { get; set; }
        #endregion

        #region Constructors
        public Section()
        {
            Facts = new List<Fact>();
        }
        #endregion
    }
}
