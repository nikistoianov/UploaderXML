# Convert XML to JSON
A small app for uploading XML files, converting them to JSON and saving them to desired directory

### UI usage:
   ![](/images/ui.jpg)
1. Select XML file to upload
2. Click Upload button
3. Result will appear in the text box

### API usage:
1. Endpoint used for uploading and converting XML file is `/api/Files/ConvertXmlToJson` with **POST** request
   ![](/images/api1.jpg)
2. Fill in the desired parameters
   ![](/images/api2.jpg)
 - ***filePath*** - desired location to save the converted JSON file, if not provided the current directory will be used
 - ***overwriteExistingFile*** - if a JSON file already exists on the server and the supplied value is true, the file will be overwritten
 - ***file*** - attached XML file (**required**)
3. After sending the request, in case of successful conversion and saving, the json text is returned as a result. If it is not successful the application will return a
meaningful error message
