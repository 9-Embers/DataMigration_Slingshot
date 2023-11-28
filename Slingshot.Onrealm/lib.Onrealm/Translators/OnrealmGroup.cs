using lib.Onrealm.Contracts;
using Slingshot.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.Onrealm.Translators;
public static class OnrealmGroup
{
    public static Slingshot.Core.Model.Group Translate(lib.Onrealm.Contracts.Group inputGroup, int campusId)
    {
        Slingshot.Core.Model.Group group = new();

        //group.Id = inputGroup.GroupId;
        group.Name = inputGroup.GroupName;
        group.Description = inputGroup.Description;
        //group.GroupTypeId = inputGroup.GroupType;
        group.CampusId = campusId;
        group.IsActive = !inputGroup.DateDeactivated.HasValue;
        group.Order = inputGroup.SortOrder ?? 0;



        return group;
    }

}
