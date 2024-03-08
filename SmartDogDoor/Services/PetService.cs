﻿namespace SmartDogDoor.Services;
using System.Net.Http.Json;
using System;
using Microsoft.Data.SqlClient;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Identity;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;
using CommunityToolkit.Maui.Converters;
using Android.Telephony;
using static Java.Util.Jar.Attributes;



//Class for accessing outside data from pet server
public class PetService
{
    HttpClient httpClient;
    public PetService()
    {
        httpClient = new HttpClient();
    }

    //Lists for entries from database tables
    List<Pet> petList = new ();
    List<PetActivity> petActivityList = new();
    List<Lock> lockList = new();
    //BlobContainerClient bloby;

    //Function to get data from Pet Information Database Table
    public async Task<List<Pet>> GetPets()
    {
        petList.Clear();//clear current pet list

        var url = "https://petconnect.azurewebsites.net/api/petInfo";

        var response = await httpClient.GetAsync(url);
        //use async for webapi calls
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
            petList = await response.Content.ReadFromJsonAsync<List<Pet>>();
        }

        return petList;
    }

    //Function to return local list of pets
    public List<Pet> GetPetsLocal()
    {
        return petList;
    }



    //Function to get data from Pet Information Database Table
    public async Task ChangePetName(string id, string name)
    {
        var url = $"https://petconnect.azurewebsites.net/api/pet/{id}/name/{name}";
        Console.Write($"Request Url: {url}");
        var response = await httpClient.PutAsync(url,null);
        //use async for webapi calls
        Console.Write(response);
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
        }

        foreach (var pet in petList)
        {
            if (pet.Id == id)
            {
                pet.Name = name;
            }
        }

        return;
    }
    /*
    public async void addPet()
    {
        //connect to database

        //read number of entries in pet information database table

        //add a new row to database table and make the pet name Pet<numofPets+1> and make inOut True

    }
    */

    /*
    public async void deletePet(string id)
    {
        //connect to database

        //read number of entries in pet information database table

        //add find row with passed id in pet information database table and delete pet
        //find entries in pet activity database table with passed id and delete them
    }
    */

    /*
   public async void changePetName(string id, string name)
   {
       //connect to database

       //find entry in pet information database table with passed id

       //change found entry with passed id's name to passed name
   }
   */

    public async Task<String> addPetImageDatabase(String imagePath, String fileName, string petName)
    {
        var url = "https://petconnect.azurewebsites.net/api/Files";//api url        

        var multipartContent = new MultipartFormDataContent();
        var saveFilename = "";
        /*
        var file = new FileInfo(imagePath);
        if (!file.Exists)
            throw new ArgumentException($"Unable to access file at: {imagePath}", nameof(imagePath));
        */
        
        var file = new ByteArrayContent(File.ReadAllBytes(imagePath)) ?? throw new ArgumentException($"Unable to access file at: {imagePath}", nameof(imagePath));

        Debug.WriteLine(fileName);
        var index = fileName.LastIndexOf('.');
        var fileType = fileName.Substring(index, fileName.Length - index - 1);
        Debug.WriteLine(index);
        if (fileType == "png")
        {
            file.Headers.Add("Content-Type", "image/png");
            saveFilename = $"Profile_{petName}.png";
        }
        else
        {
            file.Headers.Add("Content-Type", "image/jpeg");
            saveFilename = $"Profile_{petName}.jpg";
        }
           
        multipartContent.Add(file, "file", saveFilename);

        //send command
        var response = await httpClient.PostAsync(url, multipartContent);
        response.EnsureSuccessStatusCode(); // this throws an exception on non HTTP success codes

        return saveFilename;
    }

    public async Task<String> changePetImage(string id, string filename)
    {
        var url = $"https://petconnect.azurewebsites.net/api/pet/{id}/image/{filename}";
        Console.Write($"Request Url: {url}");
        var response = await httpClient.PutAsync(url, null);
        //use async for webapi calls
        Console.Write(response);
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
        }

        var newImage = $"https://petimagestorage.blob.core.windows.net/pet-images/{filename}";
        foreach (var pet in petList)
        {
            if (pet.Id == id)
            {
                pet.Image = newImage;
            }
        }

        return newImage;
    }


    /*
    public async void deletePetImages(string ID)
    {
        //connect to database and Blob storage

        //search pet information database table and pet activity database table for all entries for passed ID and get all 
        //entries image URLs

        //Using URL for images go into blob storage and delete all images with the received URLS
    }
    */


    public async Task deletePetImage(string image)
    {
        //Debug.WriteLine(image);
        var index = image.LastIndexOf('/');
        //Debug.WriteLine(index);
        if (index == -1)
            return;
        //Debug.WriteLine(image.Length);
        //Debug.WriteLine(image.Length-index);
        image = image.Substring(index+1, image.Length-index-1);
        //Debug.WriteLine(image);

        var url = $"https://petconnect.azurewebsites.net/api/Files/filename?filename={image}";//api url        

        //send command
        var response = await httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode(); // this throws an exception on non HTTP success codes

        return;
    }
    

    //Not Completed
    //Function for getting entries from Pet Activity database table.
    public async Task<List<PetActivity>> GetPetActivities()
    {
        petActivityList.Clear();//clear current pet list

        //Eventually add check for new activity 

        var url = "https://petconnect.azurewebsites.net/api/petActivity";

        var response = await httpClient.GetAsync(url);
        //use async for webapi calls
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
            petActivityList = await response.Content.ReadFromJsonAsync<List<PetActivity>>();

            //Get InOut Colors
            foreach (var activity in petActivityList)
            {
                //Get InOut Colors
                if (activity.InOut == "True")
                { 
                    activity.InOut = "In";
                    activity.InOutColor = Color.FromRgba("#008450");
                }
                else
                {
                    activity.InOut = "Out";
                    activity.InOutColor = Color.FromRgba("#B81D13");
                }
                    
            }

            //Get Pet Names
            foreach (var activity in petActivityList)
            {
                //Get Pet Names
                foreach(var pet in petList)
                {
                    if(activity.Id == pet.Id)
                    {
                        activity.Name = pet.Name;
                    }
                }
            }

            //Convert Epoch Time to DateTime
            foreach (var activity in petActivityList)
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(activity.TimeStamp));
                dateTimeOffset = dateTimeOffset.ToLocalTime();
                activity.TimeStamp = dateTimeOffset.DateTime.ToString();
            }
        }

        return petActivityList;
    }

    //Not Completed
    //Function for getting entries from locking restriction database table.
    public async Task<List<Lock>> GetLocks()
    {
        lockList.Clear();//clear current pet list

        //Eventually add check for new activity 

        var url = "https://petconnect.azurewebsites.net/api/petActivity";

        var response = await httpClient.GetAsync(url);
        //use async for webapi calls
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
            lockList = await response.Content.ReadFromJsonAsync<List<Lock>>();
        }

        return lockList;

    }

    /*
    public async void addLock(string timeStart, string timeStop, bool lockUnlock)
    {
        //connect to database

        //add entry to locking restriction database table with passed parameters

    }
    */

    /*
    public async void deleteLock(Lock lock)
    {
        //connect to database

        //delete entry of locking restriction database table with has entires same as the members of passed Lock object
    }
    */

}


