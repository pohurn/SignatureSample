using SignatureApp.ViewModels;
using System.Diagnostics;

namespace SignatureApp.Views;

public partial class SignaturePage : ContentPage
{
    SignatureViewModel viewmodel;
    bool canClickonSaveBtn = false;
    string signatureType;

    public SignaturePage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        viewmodel = new SignatureViewModel(Navigation);
        BindingContext = viewmodel;
    }


    private void SignatureStarted(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Savebtn.IsVisible = true;
    }

    private async void SaveSignatureClicked(object sender, EventArgs e)
    {
        if (canClickonSaveBtn) return;
        canClickonSaveBtn = true;

        string image = await GetPhotoBase64Async();

        //if (!string.IsNullOrEmpty(image))
        //{
        //    await viewmodel.PostSignature(image, signatureType);
        //}

        canClickonSaveBtn = false;
    }

    private void ClearSignatureClicked(object sender, EventArgs e)
    {
        signaturePad.Clear();
        Savebtn.IsVisible = false;
    }

    public async Task<string> GetPhotoBase64Async()
    {
        ImageSource imageSource = signaturePad.ToImageSource();
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string fileName = System.IO.Path.GetRandomFileName() + ".png"; // Provide a desired file name
        string filePath = System.IO.Path.Combine(folderPath, fileName);

        byte[] imageData;

        if (imageSource is FileImageSource fileImageSource)
        {
            string imagePath = fileImageSource.File;
            imageData = File.ReadAllBytes(imagePath);
        }
        else if (imageSource is StreamImageSource streamImageSource)
        {
            using (Stream stream = await streamImageSource.Stream.Invoke(new CancellationToken()))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }
        }
        else
        {
            // Handle other types of ImageSource as needed
            return null;
        }

        File.WriteAllBytes(filePath, imageData);
        byte[] bytes = File.ReadAllBytes(filePath);
        string base64 = Convert.ToBase64String(bytes);
        Debug.WriteLine($"Signature base64: {base64}");
        return base64;
    }
}