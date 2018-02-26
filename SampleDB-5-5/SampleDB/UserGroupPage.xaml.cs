using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Xamarin.Forms;
using static SampleDB.UserGroupPageViewModel;

namespace SampleDB
{
    public partial class UserGroupPage : ContentPage
    {
        public UserGroupPage()
        {
            InitializeComponent();
            this.BindingContext = new UserGroupPageViewModel();




            //var assembly = typeof(DynamicScreen).GetTypeInfo().Assembly;
            //Stream stream = assembly.GetManifestResourceStream("SampleDB.UserGroup.json");
            //using (var reader = new System.IO.StreamReader(stream))
            //{

            //    var json = reader.ReadToEnd();
            //    UserGroupModel userGroupModel = JsonConvert.DeserializeObject<UserGroupModel>(json);

            //    List<Group> sortingGroup = new List<Group>();
            //    for (int i = 0; i < userGroupModel.Groups.Count; i++){
            //        if (userGroupModel.Groups[i].GroupType.Level_Id == 2){
            //            sortingGroup.Add(userGroupModel.Groups[i]);
            //        }
            //    }

            //    pickerRef.ItemsSource = sortingGroup;
            //}
        }

        public void OnMore(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            JSSEInfo obj = mi.CommandParameter as JSSEInfo;
            DisplayAlert("", obj.jsseId.ToString(), "OK");
        }

        public void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            JSSEInfo obj = mi.CommandParameter as JSSEInfo;
            DisplayAlert("",obj.jsseId.ToString(), "OK");
        }
    }
}
