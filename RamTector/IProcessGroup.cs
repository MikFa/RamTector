using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RamTector
{
    public interface IProcessGroup : IWatchedProcess
    {
        string GroupName { get;  }
        List<IProcessGroupMember> GroupMembers { get; set; }
    }
}
