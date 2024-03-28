namespace SmartDogDoor.Services;
using System.Net.Http.Json;
using System;
using Microsoft.Data.SqlClient;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Identity;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using CommunityToolkit.Maui.Converters;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Plugin.LocalNotification;



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
    List<PetActivity> petActivityListRaw = new(); // for keeping timestamps in orginial form
    List<Lock> lockList = new();
    

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
    
    public async Task addPet()
    {
        var newPet = new { id = "0", name = "", image = "", inOut = ""};
        var jsonContent = JsonConvert.SerializeObject(newPet);

        //string jsonPetInfo = $"{{\"id\": \"0\", \"name\": \"{petList.Count + 1}\",  \"image\": \"\",  \"inOut\": \"In\"}}";

        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //var httpContent = new StringContent(jsonPetInfo);
        var url = $"https://petconnect.azurewebsites.net/api/addPet";
        Console.Write($"Request Url: {url}");
        var response = await httpClient.PostAsync(url, httpContent);
        //use async for webapi calls
        Console.Write(response);
        if (response.IsSuccessStatusCode)
        {
            Debug.WriteLine(response.Content);
        }

        Debug.WriteLine("Added Pet");

        return;
    }

    
    // check for pet id of zero to see if pet has been successfully added
    public async Task<bool> checkForPetIdOfZero()
    {
        var url = "https://petconnect.azurewebsites.net/api/petId";
        List<Pet> Ids = new();

        var response = await httpClient.GetAsync(url);
        //use async for webapi calls
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
            Ids = await response.Content.ReadFromJsonAsync<List<Pet>>();

            foreach(var pet in Ids)
            {
                if (pet.Id == "0")
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    // check for pet id of zero to see if pet has been successfully added
    public async Task<bool> checkForNewId(int petCount)
    {
        var url = "https://petconnect.azurewebsites.net/api/petId";
        List<Pet> Ids = new();

        var response = await httpClient.GetAsync(url);
        //use async for webapi calls
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
            Ids = await response.Content.ReadFromJsonAsync<List<Pet>>();

            if (Ids.Count > petCount)
            {
                return true;
            }
        }

        return false;
    }

    // Delete Pet Entry from Pet Information Table
    public async Task deletePet (string id)
    {
        var url = $"https://petconnect.azurewebsites.net/api/deletePet/{id}";
        Console.Write($"Request Url: {url}\n");
        var response = await httpClient.DeleteAsync(url);
        //use async for webapi calls
        Console.Write(response);
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
        }
    }
    
    //Delete all Images and Entries associated with a given pet
    public async Task deleteAllPetInformation(string id)
    {

        await deleteAllPetImages(id);

        foreach(var activity in petActivityListRaw)
        {
            if (activity.Id  == id)
            {
                await deletePetActivity(activity.TimeStamp);
            }
        }

        await deletePet(id);

        return;
    }

    // Delete an entry in the pet activity table
    public async Task deletePetActivity(string timestamp)
    {
        // Fix



        var url = $"https://petconnect.azurewebsites.net/api/deletePetActivity/{timestamp}";
        Console.Write($"Request Url: {url}\n");
        var response = await httpClient.DeleteAsync(url);
        //use async for webapi calls
        Console.Write(response);
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
        }
    }

    // Add new pet image to database (blob storage)
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
            if(petName.IndexOf(".png") != -1)
            {
                saveFilename = $"Profile_{petName}";
            }
            else
            {
                saveFilename = $"Profile_{petName}.png";
            }
            
        }
        else
        {
            file.Headers.Add("Content-Type", "image/jpeg");
            if (petName.IndexOf(".jpg") != -1)
            {
                saveFilename = $"Profile_{petName}";
            }
            else
            {
                saveFilename = $"Profile_{petName}.jpg";
            }
        }
           
        multipartContent.Add(file, "file", saveFilename);

        //send command
        var response = await httpClient.PostAsync(url, multipartContent);
        response.EnsureSuccessStatusCode(); // this throws an exception on non HTTP success codes

        return saveFilename;
    }

    // Change Pet Profile Image for passed pet in database
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

    public async Task deleteAllPetImages(string Id)
    {
        
        foreach(var activity in petActivityList)
        {
            if (activity.Id == Id)
            {
                await deletePetImage(activity.Image);
            }
        }

        foreach (var pet in petList)
        {
            if (pet.Id == Id)
            {
                if (pet.Image != "")
                    await deletePetImage(pet.Image);
            }
        }
    }

    // delete specified image in database (blob storage)
    public async Task deletePetImage(string image)
    {
        //check if image is currently in database before deleteing it.

        var index = image.LastIndexOf('/');
        if (index == -1)
            return;
        image = image.Substring(index+1, image.Length-index-1);;

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
        int numberOldActivity = petActivityList.Count();
        petActivityList.Clear();//clear current pet list

        //Eventually add check for new activity 

        var url = "https://petconnect.azurewebsites.net/api/petActivity";

        var response = await httpClient.GetAsync(url);
        //use async for webapi calls
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
            petActivityList = await response.Content.ReadFromJsonAsync<List<PetActivity>>();
            petActivityListRaw = petActivityList;
            var numberNewActivity = petActivityList.Count() - numberOldActivity;

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

            // Send Notifications (Possibly update to include pet image in notification this may be difficult....)
            if (numberOldActivity != 0)
            {
                for (int i = 0; i < numberNewActivity; i++)
                {
                    //if (!LocalNotificationCenter.Current.AreNotificationsEnabled().Result)
                    //{
                    //    await LocalNotificationCenter.Current.RequestNotificationPermission();
                    //}
                    var request = new NotificationRequest
                    {
                        NotificationId = (1000 + i),
                        Title = "Pet Activity Detected",
                        //Subtitle = "Hello Friends",
                        Description = $"{petActivityList[petActivityList.Count() - numberNewActivity].Name} detected going {petActivityList[petActivityList.Count() - numberNewActivity].InOut} at {petActivityList[petActivityList.Count() - numberNewActivity].TimeStamp}",
                        //Image = $"{petActivityList[petActivityList.Count() - numberNewActivity].Image}",
                        BadgeNumber = 42,
                        /*Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = DateTime.Now,
                        }*/
                    };
                    await LocalNotificationCenter.Current.Show(request);
                }
            }
        }

        //Debug.WriteLine("Notification Enabled?");
        //Debug.WriteLine(LocalNotificationCenter.Current.AreNotificationsEnabled().Result);

        return petActivityList;
    }

    //Not Completed
    //Function for getting entries from locking restriction database table.
    public async Task<List<Lock>> GetLocks()
    {
        lockList.Clear();//clear current pet list

        //Eventually add check for new activity 

        var url = "https://petconnect.azurewebsites.net/api/lockRestriction";

        var response = await httpClient.GetAsync(url);
        //use async for webapi calls
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
            lockList = await response.Content.ReadFromJsonAsync<List<Lock>>();
        }

        return lockList;

    }

    
    public async Task addLock(Lock restriction)
    {
        var newLock = new { id = restriction.Id, timeStartDay = restriction.TimeStartDay, timeStartHour = restriction.TimeStartHour, timeStartMinute = restriction.TimeStartMinute, timeStopDay = restriction.TimeStopDay, timeStopHour = restriction.TimeStopHour, timeStopMinute = restriction.TimeStopMinute };
  
        var jsonContent = JsonConvert.SerializeObject(newLock);

        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //var httpContent = new StringContent(jsonPetInfo);
        var url = $"https://petconnect.azurewebsites.net//api/addLockRestriction";
        Console.Write($"Request Url: {url}");
        var response = await httpClient.PostAsync(url, httpContent);
        //use async for webapi calls
        Console.Write(response);
        if (response.IsSuccessStatusCode)
        {
            Debug.WriteLine(response.Content);
        }

        Debug.WriteLine("Added Pet");

        return;
    }
    

   
    public async Task deleteLock(int id)
    {
        var url = $"https://petconnect.azurewebsites.net/api/deleteLockRestriction/{id}";
        Console.Write($"Request Url: {url}\n");
        var response = await httpClient.DeleteAsync(url);
        // use async for webapi calls
        Console.Write(response);
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
        }
    }

}


