using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Tmds.DBus.Protocol;

namespace session12;

public partial class Catalog : Window
{
    public List<Product> filteredProds = new List<Product>();
    public List<Product> prodsManufacturer = new List<Product>();
    public int a = 0;
    public Catalog()
    {
        InitializeComponent();
        switch (Info.codeRole)
        {
            case 0://adm
                addTovarBtn.IsVisible = true;
                break;
            case 1:
                addTovarBtn.IsVisible = false;
                break;
            case 2:
                addTovarBtn.IsVisible = false;
                break;
        }
       

        ProductsList.ItemsSource = Info.products;
        List<string> manufacturers = new List<string>();
        manufacturers.Add("Снять сортировку");


        List<int> manindex = new List<int>();
        for (int i = 0; i < Info.products.Count-1; i++)
        {
            for (int j = i+1; j < Info.products.Count; j++)
            {
                if (Info.products[i].manufacturer == Info.products[j].manufacturer)
                {
                    manindex.Add(j);
                }
            }
        }
        foreach (Product prod in Info.products)
        {
            manufacturers.Add(prod.manufacturer);
        }
        if (manindex.Count > 0)
        {
            foreach (int manind in manindex)
            {
                if (manind != manufacturers.Count - 1)
                {
                    manufacturers.RemoveAt(manind + 1);
                }
                else
                {
                    manufacturers.RemoveAt(manind);
                }
            }
        }
        kolvoTovarov.Text = Info.products.Count.ToString() + "/" + Info.products.Count.ToString(); 
        manufacturersComboBox.ItemsSource = manufacturers.ToList();
    }

    private void AddProduct_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AddProduct addProduct = new AddProduct();
        addProduct.Show();
        this.Close();
        
    }
    private void VyhodBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        Info.codeRole = -1;
        this.Close();
    }

    private void manufacturer_Select(object sender, SelectionChangedEventArgs e)
    {
        prodsManufacturer.Clear();
        if (manufacturersComboBox.SelectedIndex > 0 && priceFilter.SelectedIndex > 0)//у производителей есть фильтр, на цене тоже
        {
            foreach (Product prod in filteredProds)
            {
                if (prod.manufacturer == manufacturersComboBox.SelectedValue.ToString())
                {
                    prodsManufacturer.Add(prod);
                }
            }
            ProductsList.ItemsSource = prodsManufacturer.ToList();
            kolvoTovarov.Text = prodsManufacturer.Count.ToString() + "/" + Info.products.Count.ToString();
        }
        else if (manufacturersComboBox.SelectedIndex > 0 && (priceFilter.SelectedIndex == -1 || priceFilter.SelectedIndex == 0)) //на проивзодителях есть фильтр, на цене нет
        {
            foreach (Product prod in Info.products)
            {
                if (prod.manufacturer == manufacturersComboBox.SelectedValue.ToString())
                {
                    prodsManufacturer.Add(prod);
                }
            }
            ProductsList.ItemsSource = prodsManufacturer.ToList();
            kolvoTovarov.Text=prodsManufacturer.Count.ToString() + "/" + Info.products.Count.ToString();
        }
        else if (manufacturersComboBox.SelectedIndex == 0 && priceFilter.SelectedIndex > 0)//на производителях нет фильтра, на цене есть
        {
            ProductsList.ItemsSource = filteredProds.ToList();
            kolvoTovarov.Text = filteredProds.Count.ToString() + "/" + Info.products.Count.ToString();
        }
        else if (manufacturersComboBox.SelectedIndex == 0 && priceFilter.SelectedIndex == 0)// на производителях нет фильтра, и на цене нет фильтра
        {
            ProductsList.ItemsSource = Info.products.ToList();
            kolvoTovarov.Text = Info.products.Count.ToString() + "/" + Info.products.Count.ToString();
        }
        else
        {
            ProductsList.ItemsSource = Info.products.ToList();
            kolvoTovarov.Text = Info.products.Count.ToString() + "/" + Info.products.Count.ToString();
        }
        
/*

        if (priceFilter.SelectedIndex != -1 && manufacturersComboBox.SelectedIndex != -1)
        {
            foreach (Product prod in filteredProds)
            {
                if (prod.manufacturer == manufacturersComboBox.SelectedValue.ToString())
                {
                    prodsManufacturer.Add(prod);
                }
            }
            ProductsList.ItemsSource = prodsManufacturer.ToList();
        }
        else if (manufacturersComboBox.SelectedIndex != -1 && priceFilter.SelectedIndex == -1)
        {
            foreach (Product prod in Info.products)
            {
                if (prod.manufacturer == manufacturersComboBox.SelectedValue.ToString())
                {
                    prodsManufacturer.Add(prod);
                }
            }
            ProductsList.ItemsSource = prodsManufacturer.ToList();
        }
        else if (priceFilter.SelectedIndex != -1 && manufacturersComboBox.SelectedIndex == 0)
        {
            ProductsList.ItemsSource = Info.products.ToList();
        }
        else
        { 
            
        }*/
    }

    private void AddToKorzBtn_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        bool checkRepeat = true;

        int ind = (int)((sender as Button)!).Tag!;

        for (int i = 0; i < Info.productsInKorzina.Count; i++)
        {
            if (Info.products[ind].name == Info.productsInKorzina[i].name)
            {
                checkRepeat = false;
                MessageBoxManager.GetMessageBoxStandard(text: "Этот товар уже содержится в корзине", title: "Уведомление").ShowAsync();
                break;
            }
        }


        if (!checkRepeat)
        {
            if (Info.productsInKorzina.Count > 0)
            {
                Info.productsInKorzina.Add(
                    new ProductKorz()
                    {
                        id = a,
                        name = Info.products[ind].name,
                        price = Info.products[ind].price,
                        manufacturer = Info.products[ind].manufacturer,
                        units = Info.products[ind].units,
                        quantity = Info.products[ind].quantity,
                        description = Info.products[ind].description,
                        image = Info.products[ind].image,
                        quantitySelect = 1
                    });

                a = 0;
                foreach (ProductKorz prSel in Info.productsInKorzina)
                {
                    prSel.id = a;
                    a++;
                }

            }
           
            a++;
        }
    }

    private void checkQuantityProds()
    { 
       // foreach
    
    
    
    
    }

    private void KorzinaBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
      /*  List<Product> prods = new List<Product>();
        prods = ProductsList.SelectedItems.Cast<Product>().ToList();*/

        Korzina korzinanew = new Korzina();
        korzinanew.Show();
        Close();
    }




    private void DelBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int ind = (int)((sender as Button)!).Tag!;

        bool checkTovarInKorz = false;
        foreach (ProductKorz prK in Info.productsInKorzina)
        {
            if (prK.name == Info.productsInKorzina[ind].name)
            {
                checkTovarInKorz = true;
                // Info.productsInKorzina.Remove(prK);
                MessageBoxManager.GetMessageBoxStandard(text: "Товар с именем " + prK.name + "содержится в корзине. Необходимо сначала удалить его из корзины.", title:"Уведомление").ShowAsync();
                
                break;
            }
        }
        if (!checkTovarInKorz)
        {
            Info.b = 0;
            foreach (Product pr in Info.products)
            {
                pr.id = Info.b;
                Info.b++;
            }
            Info.products.RemoveAt(ind);
            Info.b = 0;
            if (Info.products.Count > 0)
            {
                Info.b = 0;
                foreach (Product pr in Info.products)
                {
                    pr.id = Info.b;
                    Info.b++;
                }
            }
            ProductsList.ItemsSource = Info.products.ToList();
        }
        kolvoTovarov.Text = Info.products.Count.ToString() + "/" + Info.products.Count.ToString(); 
    }
   

    
    private void selectToEdit_Click(object sender, SelectionChangedEventArgs e)
    {
        Info.prodForEdit = ProductsList.SelectedIndex;
        EditItem editItem = new EditItem();
        editItem.Show();
      //  ProductsList.ItemsSource = Info.products.ToList();
       // this.Close();
    }

    private void price_Select(object sender, SelectionChangedEventArgs e)
    {
        if (priceFilter.SelectedIndex > 0 && manufacturersComboBox.SelectedIndex > 0)//на цене установлена сортировка и на производителях
        {
            filteredProds = prodsManufacturer;
            switch (priceFilter.SelectedIndex)//выбарнная сортировка
            {
                case 1:
                    Sorting(1);
                    break;
                case 2:
                    Sorting(2);
                    break;
            }
        }
        else if (priceFilter.SelectedIndex > 0 && (manufacturersComboBox.SelectedIndex == -1 || manufacturersComboBox.SelectedIndex == 0))// на цене установлена сортировка, а фильтр производителей пуст
        {
            filteredProds = Info.products;//берем полный список продуктов
            switch (priceFilter.SelectedIndex)
            {
                case 1:
                    Sorting(1);
                    break;
                case 2:
                    Sorting(2);
                    break;
            }
        }
        else if (priceFilter.SelectedIndex == 0 && manufacturersComboBox.SelectedIndex > 0)//на цене фильтр снять фильтр, на проивзодителях есть фильтр
        {
            ProductsList.ItemsSource = prodsManufacturer.ToList();//ничего не меняется, выводим только фильтр производителей
        }
        else if (priceFilter.SelectedIndex == 0 && manufacturersComboBox.SelectedIndex == 0)//на цене снять фильтр и на проивзодителях
        {
            ProductsList.ItemsSource = Info.products.ToList();
        }
        else
        {
            ProductsList.ItemsSource = Info.products.ToList();
        }
       
     }

    private void Sorting(int value)
    {
        switch (value)
        {
            case 1:
                for (int i = 0; i < filteredProds.Count - 1; i++)
                {
                    for (int j = i + 1; j < filteredProds.Count; j++)
                    {
                        if (filteredProds[i].price > filteredProds[j].price)
                        {
                            Product z = filteredProds[i];
                            filteredProds[i] = filteredProds[j];
                            filteredProds[j] = z;
                        }
                    }
                }
                ProductsList.ItemsSource = filteredProds.ToList();
                break;
            case 2:
                for (int i = 0; i < filteredProds.Count - 1; i++)
                {
                    for (int j = i + 1; j < filteredProds.Count; j++)
                    {
                        if (filteredProds[i].price < filteredProds[j].price)
                        {
                            Product z = filteredProds[i];
                            filteredProds[i] = filteredProds[j];
                            filteredProds[j] = z;
                        }
                    }
                }
                ProductsList.ItemsSource = filteredProds.ToList();
                break;
        }
        
    }

    private void Poisk_OnKeyUp(object? sender, KeyEventArgs e)
    {
       List<Product> finalMatch = new List<Product>();
        List<Product> matchPr = new List<Product>();
        if (poisk.Text.Length>0)
        {
            string value = poisk.Text;
            int count = value.Split(' ').Length;
            string[] values = new string[count];
           
            values = value.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in values)
            {
                foreach (Product pr in Info.products)
                {
                    if (pr.name.Contains(s) || pr.units.ToString().Contains(s) || pr.price.ToString() == s
                  || pr.manufacturer.Contains(s) || pr.category == s || pr.description.Contains(s)
                  || pr.quantity.ToString() == s)
                    {

                        matchPr.Add(pr);
                    }

                }
            }

            List<int> countMatch = new List<int>();
            /* for (int i = 0; i < matchPr.Count; i++)
             {
                if (i == 0 || matchPr[i] != matchPr[i - 1])
                {
                    countMatch.Add(matchPr.Where(x => x == matchPr[i]).Count());
                }
                else if (matchPr[i] == matchPr[i - 1])
                {
                        i++;
                }
             }*/

            /*int maxval = countMatch.Max();

            int ind = countMatch.IndexOf(maxval);*/
             int count2 = 0;
            for (int i = 0; i < matchPr.Count; i++)
            {
                count2 = 0;
                countMatch.Add(count2);

                for (int j = i + 1; j < matchPr.Count; j++)
                {
                    if (matchPr[i] == matchPr[j] && i != j)
                    {
                        countMatch[i]++;
                    }
                }
            }

            for (int i = 0; i < countMatch.Count - 1; i++)
            {
                for (int j = i + 1; j < countMatch.Count; j++)
                {
                    if (countMatch[i] > countMatch[j])
                    {
                        int temp = countMatch[i];
                        countMatch[i] = countMatch[j];
                        countMatch[j] = temp;

                        Product pr = matchPr[i];
                        matchPr[i] = matchPr[j];
                        matchPr[j]  = pr;
                    }
                }
            }



            for (int i = countMatch.Count - 1; i >= 0; i--)
            {
                if (i == countMatch.Count - 1 || (countMatch[i] == countMatch[i + 1] && i < countMatch.Count - 1))
                {
                    finalMatch.Add(matchPr[i]);
                }
                else
                {
                    break;
                }
            }



            /* for (int i = countMatch.Count - 1; i >= 0; i--)
             { 
             if ()

             }*/

            // for (int j = i+1; j < matchPr.Count; j++)
            //{
            // if (matchPr[i] == matchPr[j])
            //{ 

            //   foreach (Product match in matchPr)
            //  {

            //  }


            //  }
            ProductsList.ItemsSource = finalMatch.ToList();
            //}
        }
        else if (poisk.Text.Length==0)
        { 
            ProductsList.ItemsSource = Info.products.ToList();
        }


            
            /* foreach (Product pr in Info.products)
             {

                 if (pr.name.Contains(value))
                 {
                     matchPr.Add(pr);
                 }
                 else if (pr.manufacturer.Contains(value))
                 {
                     matchPr.Add((Product)pr);
                 }
                 else if (pr.units.Contains(value))
                 {
                     matchPr.Add(pr);
                 }
                 else if (pr.price.Contains)
             }*/
        
          //  ProductsList.ItemsSource = Info.products.ToList();

        
        

    }
    
   
    /* 
     * ListBox1.DoubleClick += new EventHandler(ListBox1_DoubleClick);
     * void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
     {
         int index = this.listBox1.IndexFromPoint(e.Location);
         if (index != System.Windows.Forms.ListBox.NoMatches)
         {
             MessageBox.Show(index.ToString());
         }
     }*/


}