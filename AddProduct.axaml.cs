using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System;
using System.Linq;
using MsBox.Avalonia;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace session12;

public partial class AddProduct : Window
{
    public List<string> categories { get; set; }
    public AddProduct()
    {
        categories = new List<string>()
        {
        new string ("1"),
        new string ( "2"),
        new string ("3"),
        new string ( "4"),
        };
        InitializeComponent();
        categoriesComboBox.ItemsSource = categories.ToList();
    }

    private void AddOk_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Product product = new Product();
        product.name = namePr.Text;
        product.category = categories.IndexOf(categoriesComboBox.SelectedIndex.ToString()).ToString();
        product.manufacturer = manufPr.Text;
       
        product.price = Convert.ToDouble(pricePr.Text);

        product.units = unitsPr.Text;
        product.description = descrPr.Text;
        product.id = Info.ind++;
        product.image = bitmapToBind;

        product.quantity = Convert.ToInt32(kolvoPr.Text);
        if (checkValues(Convert.ToDouble(pricePr.Text), Convert.ToInt32(kolvoPr.Text)) == false)
        {
            return;
        }
        else
        {
            Info.products.Add(product);
            this.Close();
        }
       // Catalog catalog = new Catalog();
      //  catalog.Show();
        
    }
    private bool checkValues(double pr, int quan)
    {
        bool check = true;

        if (pr > 0)
        {
            check = true;
        }
        else
        {
            check = false;
            pricePr.Text = "";
            MessageBoxManager.GetMessageBoxStandard(text: "���� ������ �� ����� ���� �������������. ������� ������.", title: "������").ShowAsync();
        }


        if (quan < 0)
        {
            check = false;
            kolvoPr.Text = "";
            MessageBoxManager.GetMessageBoxStandard(text: "���������� ������ �� ����� ���� �������������. ������� ������.", title: "������").ShowAsync();
        }
        return check;
    }
    
    public string path;
    public Bitmap bitmapToBind;

    private async void AddTovarImg_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        OpenFileDialog _openFileDialog = new OpenFileDialog();
        var result = await _openFileDialog.ShowAsync(this);
        if (result == null) return;
        string path = string.Join("", result);
        if (result != null)
        {
            bitmapToBind = new Bitmap(path);
        }
        imageAdd.Source = bitmapToBind;

    }

}