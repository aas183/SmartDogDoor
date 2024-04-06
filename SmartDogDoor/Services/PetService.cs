namespace SmartDogDoor.Services;
using System.Net.Http.Json;
using System;
using System.Text;
using Newtonsoft.Json;
using Plugin.LocalNotification;
using System.Diagnostics;



//Class for accessing outside data from pet server
public class PetService
{
    HttpClient httpClient;
    public PetService()
    {
        httpClient = new HttpClient();
    }

    const int MAX_ACTIVITY = 100;
    //Lists for entries from database tables
    List<Pet> petList = new ();
    List<PetActivity> petActivityList = new();
    //List<PetActivity> petActivityListRaw = new(); // for keeping timestamps in orginial form
    List<Lock> lockList = new();


    // Struct for checking all images
    public struct savedImages // struct for holding saved image content
    {
        String uri;
        public String name;
        String cotentType;
        String content;
    }
    

    //Function to get data from Pet Information Database Table
    public async Task<List<Pet>> GetPets()
    {
        petList.Clear();//clear current pet list

        var url = "https://petconnect.azurewebsites.net/api/petInfo";

        var response = await httpClient.GetAsync(url);
        
        // if successful get content
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



    // Function to change pet name
    public async Task ChangePetName(string id, string name)
    {
        var url = $"https://petconnect.azurewebsites.net/api/pet/{id}/name/{name}";
        Console.Write($"Request Url: {url}");
        var response = await httpClient.PutAsync(url,null);
        
        //  print response
        Console.Write(response);
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
        }

        // change pet name in petList
        foreach (var pet in petList)
        {
            if (pet.Id == id)
            {
                pet.Name = name;
            }
        }

        return;
    }
    
    // Function to Add a new Pet
    public async Task addPet()
    {
        // Configure JSON
        var newPet = new { id = "0", name = "", image = "", inOut = ""};
        var jsonContent = JsonConvert.SerializeObject(newPet);

        // Send Post command to API
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var url = $"https://petconnect.azurewebsites.net/api/addPet";
        Console.Write($"Request Url: {url}");
        var response = await httpClient.PostAsync(url, httpContent);
        
        // Check response
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
        // Configure command and data to recieve response
        var url = "https://petconnect.azurewebsites.net/api/petId";
        List<Pet> Ids = new();

        // Send command
        var response = await httpClient.GetAsync(url);
        
        // if successful check for id of zero
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
            Ids = await response.Content.ReadFromJsonAsync<List<Pet>>();

            // check for id of 0
            foreach(var pet in Ids)
            {
                if (pet.Id == "0")
                {
                    return false; // id of 0 found
                }
            }
        }

        return true; // no id of 0
    }
    
    // check for pet id of zero to see if pet has been successfully added
    public async Task<bool> checkForNewId(int petCount)
    {
        // Configure command and data to recieve response
        var url = "https://petconnect.azurewebsites.net/api/petId";
        List<Pet> Ids = new();
        
        // Send Command
        var response = await httpClient.GetAsync(url);

        // If successful check if a new pet entry
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
            Ids = await response.Content.ReadFromJsonAsync<List<Pet>>();

            if (Ids.Count > petCount)// new pet
            {
                return true;
            }
        }

        return false;// no new pet
    }

    // Delete Pet Entry from Pet Information Table
    public async Task deletePet (string id)
    {
        // Send Command
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

        await deleteAllPetImages(id); // delete all images associated with pet

        await GetPetActivities(); // get pet activities

        // loop through and delete all pet activites
        foreach(var activity in petActivityList)//petActivityListRaw
        {
            if (activity.Id  == id)
            {
                await deletePetActivity(DateTimeOffset.Parse(activity.TimeStamp).ToUnixTimeSeconds().ToString());
            }
        }

        await deletePet(id); // delete pet entry

        return;
    }

    // Delete an entry in the pet activity table
    public async Task deletePetActivity(string timestamp)
    {
        // Send Command to Delete
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
        
        // convert image stored on phone to byte array
        var file = new ByteArrayContent(File.ReadAllBytes(imagePath)) ?? throw new ArgumentException($"Unable to access file at: {imagePath}", nameof(imagePath));

        Debug.WriteLine(fileName);
        var index = fileName.LastIndexOf('.');
        var fileType = fileName.Substring(index, fileName.Length - index - 1);
        Debug.WriteLine(index);
        if (fileType == "png") // if png update saved file name to be png
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
        else  // if jpg update saved file name to be png
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

        return saveFilename; // return new filename
    }

    // Change Pet Profile Image for passed pet in database
    public async Task<String> changePetImage(string id, string filename)
    {
        var url = $"https://petconnect.azurewebsites.net/api/pet/{id}/image/{filename}";
        var response = await httpClient.PutAsync(url, null);
        
        //use async for webapi calls
        Console.Write(response);
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
        }

        // Insert new image in pet with passed pet id
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

    // Delete all images associated with a pet
    public async Task deleteAllPetImages(string Id)
    {
        // Delete all images of pet activity associated with pet
        foreach(var activity in petActivityList)
        {
            if (activity.Id == Id)
            {
                await deletePetImage(activity.Image);
            }
        }

        // Delete Pet Profile Image
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
        bool result = await imageExists(image); // check if image exists
        if (result)
        {
            //parse image name
            var index = image.LastIndexOf('/');
            if (index == -1) // image not valid
                return;
            image = image.Substring(index+1, image.Length-index-1);;

            var url = $"https://petconnect.azurewebsites.net/api/Files/filename?filename={image}";//api url        

            //send command
            var response = await httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode(); // this throws an exception on non HTTP success codes
        }

        return;
    }

    // checks if an image exists
    async private Task<bool> imageExists(string image)
    {
        // create a list of information returned from web api
        List<savedImages> Images = new List<savedImages>();

        var url = "https://petconnect.azurewebsites.net/api/Files";
        var response = await httpClient.GetAsync(url);

        //use async for webapi calls
        if (response.IsSuccessStatusCode) // if successful return;
        {
            Console.Write(response.Content);
            Images = await response.Content.ReadFromJsonAsync<List<savedImages>>();

            // Check current saved image against image name
            foreach (var savedImage in Images)
            {
                if (savedImage.name == image) // image found
                {
                    return true;
                }

            }

        }

        return false; // image not found
    }

    //Function for getting entries from Pet Activity database table.
    public async Task<List<PetActivity>> GetPetActivities()
    {
        // get old list count and clear it
        int numberOldActivity = petActivityList.Count();
        petActivityList.Clear();//clear current pet list

        var url = "https://petconnect.azurewebsites.net/api/petActivity";
        var response = await httpClient.GetAsync(url);
        
        //use async for webapi calls
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
            petActivityList = await response.Content.ReadFromJsonAsync<List<PetActivity>>();
            //petActivityListRaw = petActivityList;

            var numberNewActivity = petActivityList.Count() - numberOldActivity;

            //Get InOut Colors
            foreach (var activity in petActivityList)
            {
                //Get InOut Colors
                if (activity.InOut.ToLower() == "true")
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
                for (int i = 0; i < numberNewActivity; ++i)
                {
                    var request = new NotificationRequest
                    {
                        NotificationId = (1000 + i),
                        Title = "Pet Activity Detected",
                        //Subtitle = "None",
                        Description = $"{petActivityList[petActivityList.Count() - numberNewActivity].Name} detected going {petActivityList[petActivityList.Count() - numberNewActivity].InOut} at {petActivityList[petActivityList.Count() - numberNewActivity].TimeStamp}",
                        BadgeNumber = 42,
                    };
                    await LocalNotificationCenter.Current.Show(request);
                }
            }

            // Delete Pet Activity if there are more entries than max activity
            if(petActivityList.Count() > MAX_ACTIVITY)
            {
                for (int i = MAX_ACTIVITY; i < petActivityList.Count(); ++i)
                {
                    await deletePetActivity(DateTimeOffset.Parse(petActivityList[i].TimeStamp).ToUnixTimeSeconds().ToString());
                    petActivityList.RemoveAt(i);
                }
            }
        }

        return petActivityList;
    }


    //Function for getting entries from locking restriction database table.
    public async Task<List<Lock>> GetLocks()
    {
        lockList.Clear();//clear current lock list

        // Get lock list
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

    // Function to add locking restrictions
    public async Task addLock(Lock restriction)
    {
        // Format lock information for api
        var newLock = new { id = restriction.Id, timeStartDay = restriction.TimeStartDay, timeStartHour = restriction.TimeStartHour, timeStartMinute = restriction.TimeStartMinute, timeStopDay = restriction.TimeStopDay, timeStopHour = restriction.TimeStopHour, timeStopMinute = restriction.TimeStopMinute };
        var jsonContent = JsonConvert.SerializeObject(newLock);

        // Add Lock Information to Database
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
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
   
    // Function to delete locking restriction
    public async Task deleteLock(int id)
    {
        // Send delete command to api
        var url = $"https://petconnect.azurewebsites.net/api/deleteLockRestriction/{id}";
        Console.Write($"Request Url: {url}\n");
        var response = await httpClient.DeleteAsync(url);
        
        // get response
        Console.Write(response);
        if (response.IsSuccessStatusCode)
        {
            Console.Write(response.Content);
        }
    }

}


