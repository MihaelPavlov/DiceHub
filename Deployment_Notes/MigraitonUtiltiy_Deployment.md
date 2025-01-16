# Google Cloud Setup and Deployment Notes

### **1. Create a Google Cloud Project**

- Go to **Google Cloud Console**.
- Create a new project or select an existing one.

### **2. Set Up a Virtual Machine (VM) on Google Cloud**

- In **Google Cloud Console**, navigate to **Compute Engine** > **VM Instances**.
- Click **Create** to create a new instance.
- Choose the **Operating System** (e.g., Windows Server or a Linux distribution for .NET applications).
- Select the desired machine type and configurations.
- Allow **HTTP/HTTPS traffic** if your application requires internet access.

### **3. Install .NET Core SDK and Runtime on the VM**

Use the following commands on the VM to install .NET:

```
sudo apt-get update  
sudo apt-get install -y dotnet-sdk-5.0  
```

---

### **4. Deploy the Migration Utility to the VM**

1. Publish your C# console application as a self-contained application to include the necessary .NET runtime:
   ```
   dotnet publish -c Release -r win-x64 --self-contained
   ```
2. Transfer the publish folder to the VM:
   ```
   gcloud compute scp --recurse "C:\Users\MihaelPavlov\repos\Home\DiceHub\Utilities\DH.Database.MigrationUtility\bin\Release\net8.0\linux-x64" m_pavlov1405@migration-vm:/tmp
   ```

### **5. Connect to the VM**

Use the following command to SSH into the VM:

```
gcloud compute ssh m_pavlov1405@migration-vm --ssh-key-file C:\Users\MihaelPavlov\new_key
```

---

### **SSH Key Issues**

If there are problems with the existing SSH key, follow these steps:

#### a. **Generate a New SSH Key Pair**

Run this command to create a new key:

```
ssh-keygen -t rsa -b 2048 -f ~/.ssh/new_key
```

#### b. **Add the New Public Key to Your Google Cloud VM**

```
gcloud compute instances add-metadata migration-vm --metadata ssh-keys="your-username:$(cat ~/.ssh/new_key.pub)"
```

Replace `your-username` with your Google Cloud username.

#### c. **Use the New SSH Key**

```
gcloud compute ssh m_pavlov1405@migration-vm --ssh-key-file ~/.ssh/new_key
```

---

### **Running the Migration Utility**

1. Navigate to the `/tmp` folder where the project was uploaded.
2. Run the utility using `dotnet`:
   ```
   dotnet DH.Database.MigrationUtility.dll
   ```

---

### **Autofac Version Issue**

- **Current Problem**: The utility uses Autofac version 9.0.0, which is incompatible with your setup.
- **Solution**:
  - Downgrade Autofac to version 8.0.0.
  - Rebuild and publish the project.
  - Deploy the updated version to the VM.
-
