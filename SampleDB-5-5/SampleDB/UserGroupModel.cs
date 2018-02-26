using System;
using System.Collections.Generic;

namespace SampleDB
{
    public class User
    {
        public object CompanyId { get; set; }
        public object UserRole { get; set; }
        public int Emp_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public object UserTitle { get; set; }
        public string Email { get; set; }
        public string User_ID { get; set; }
        public string FullName { get; set; }
        public string MajorGroup_Id { get; set; }
        public string Org_Id { get; set; }
        public string Dept_Id { get; set; }
        public string Section_Id { get; set; }
        public int SecurityUserID { get; set; }
    }

    public class GroupType
    {
        public int Level_Id { get; set; }
        public object Level_Name { get; set; }
        public bool Selected { get; set; }
        public object LevelDesc { get; set; }
    }

    public class Permission
    {
        public int Permission_ID { get; set; }
        public string PermissionName { get; set; }
        public object PermissionDesc { get; set; }
        public bool Selected { get; set; }
        public bool Disabled { get; set; }
        public object PermissionLevel { get; set; }
    }

    public class Group
    {
        public int Group_ID { get; set; }
        public string GroupName { get; set; }
        public object GroupDesc { get; set; }
        public object ModifiedDate { get; set; }
        public GroupType GroupType { get; set; }
        public string MajorGroup_Id { get; set; }
        public string Org_Id { get; set; }
        public List<Permission> Permissions { get; set; }
        public object Active { get; set; }
        public object Status { get; set; }
    }

    public class UserGroupModel
    {
        public int UserGroup_ID { get; set; }
        public User User { get; set; }
        public object Users { get; set; }
        public object Group { get; set; }
        public object UserHierarchy { get; set; }
        public List<Group> Groups { get; set; }
        public object ModifiedDate { get; set; }
    }
}
