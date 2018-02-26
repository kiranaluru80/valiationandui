﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SampleDB
{
    public partial class EmployeeListPage : ContentPage
    {
        public EmployeeListPage()
        {
            InitializeComponent();
            this.Title = "Employee List";
            var toolbarItem = new ToolbarItem
            {
                Text="+"
            };
            toolbarItem.Clicked+= async (sender, e) => {
                await Navigation.PushAsync(new EmployeePage() { BindingContext = new Employee() });
            };
            ToolbarItems.Add(toolbarItem);
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            EmployeeListView.ItemsSource = await App.Database.GetEmployeeAsync();
        }
        async void Employee_ItemSelected(object sender,SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem!=null)
            {
                await Navigation.PushAsync(new EmployeePage() { BindingContext = e.SelectedItem as Employee});
            }
        }
    }
}
