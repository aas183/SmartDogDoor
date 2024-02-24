namespace SmartDogDoor.Services;
using System.Net.Http.Json;
using System;
using Microsoft.Data.SqlClient;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Identity;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;



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

    /*
    public async string addPetImage(image img)
    {
        //connect to Blob storage

        //add upload passed image into blob storage

        //return URL
    }
    */

    public static async Task UploadFromBinaryDataAsync(BlobContainerClient containerClient, string localFilePath)
    {
        string fileName = Path.GetFileName(localFilePath);
        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        FileStream fileStream = File.OpenRead(localFilePath);
        BinaryReader reader = new BinaryReader(fileStream);

        byte[] buffer = new byte[fileStream.Length];
        reader.Read(buffer, 0, buffer.Length);
        BinaryData binaryData = new BinaryData(buffer);

        await blobClient.UploadAsync(binaryData, true);

        fileStream.Close();
    }

    /*
    public async Task addPetImageDatabase(ImageSource image)
    {
        var url = "https://petconnect.azurewebsites.net/api/Files";
        Console.Write($"Request Url: {url}");
        var formContent = new MultipartFormDataContent();
        
        formContent.Add(new StreamContent(new MemoryStream(image)), "fileUpload", "fileUpload");
        var response = await httpClient.PostAsync(url, );
        //use async for webapi calls
        Console.Write(response);
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
        }

        return;
        //connect to database

        //find entry in pet information database table with passed id

        //change found entry with passed id's image link to passed URL
    }
    */

    /*
    public async void deletePetImages(string ID)
    {
        //connect to database and Blob storage

        //search pet information database table and pet activity database table for all entries for passed ID and get all 
        //entries image URLs

        //Using URL for images go into blob storage and delete all images with the received URLS
    }
    */

    /*
    public async string deletePetImage(string URL)
    {
        //connect to Blob storage

        //delete image entry with passed URL

        //return old image URL
    }
    */

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


    /*
    if(petList?.Count >  0) 
        return petList;

    var url = "";

    var response = await httpClient.GetAsync(url);
    //use asynce for webapi calls
    if(response.IsSuccessStatusCode)
    {
        petList  = await response.Content.ReadFromJsonAsync<List<Pet>>();
    }

    return petList;
    */
}


