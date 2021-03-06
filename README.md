# GraffitIT

## Project Overview
GraffitIT is a location and orientation-based content sharing mobile application that stores and shares photos, videos, and sounds with text descriptions at particular locations. GraffitIT will capture audiovisual media from the user’s phone’s built-in camera and microphone and use Unity 3D as its primary framework, which will allow for AR capabilities including placing AR objects with media content in the augmented setting and being able to view and interact with the AR objects and Unity will allow the app to build to both Android and Apple mobile devices. 

GraffitIT encourages an interactive experience while traveling to new locations. Since the media is only displayed when a user is within the same location that it was uploaded to, this leads to the experience of actively discovering content rather than simply browsing through content.
The application begins in the content sharing mode, where the user can capture content with their device, and the content type will be determined at runtime. The user created content with description is now uploaded and associated with the GPS location string generated by Unity Location Service, and the location string will be used by the Google Maps API to provide further location-related services such as display available content locations nearby the user and showing the quantity of shared contents in a given area, which helps users decide where to go when traveling. Then there is the content viewing mode where the user will be able to see other tagged contents from other users as AR objects in the augmented setting through their device camera. With the AR feature from Unity 3D, the users can tap on the AR objects to retrieve more information including the attached media content and its post description. 

Alternatively, the user can view other users’ shared media. Based on which direction they’re facing; the user could potentially see several points of interest in one geographic location. For example, if multiple users post similar contents at the approximately the same GPS coordinates, then the AR Foundation would provide the position and the orientation data on the contents recorded by various users to further differentiate where the AR objects should spawn to avoid contents overlay. 

## Contributors
Noah Williams <br>
Hadeel Alebbi <br>
Soobin Kim <br>
Mario Libohova <br>
Peter Marion <br>
Wayne Zheng <br>

## Documentation
* [GraffitIT Design Part I Architecture Document.docx](https://github.com/Capstone-Projects-2021-Fall/project-graffitit/files/7681402/GraffitIT.Design.Part.I.Architecture.Document.docx)
* [GraffitIT Design Part II API Document.docx](https://github.com/Capstone-Projects-2021-Fall/project-graffitit/files/7681403/GraffitIT.Design.Part.II.API.Document.docx)
* [GraffitIT Requirements Document.docx](https://github.com/Capstone-Projects-2021-Fall/project-graffitit/files/7681404/GraffitIT.Requirements.Document.docx)
* [GraffitIT Software Development Plan.docx](https://github.com/Capstone-Projects-2021-Fall/project-graffitit/files/7681405/GraffitIT.Software.Development.Plan.docx)
* [GraffitIT Test Report Document.docx](https://github.com/Capstone-Projects-2021-Fall/project-graffitit/files/7681406/GraffitIT.Test.Report.Document.docx)
* [Test Procedures Document.docx](https://github.com/Capstone-Projects-2021-Fall/project-graffitit/files/7681407/Test.Procedures.Document.docx)


## Release
[Link to our current release](https://github.com/Capstone-Projects-2021-Fall/project-graffitit/releases/tag/V1.2.0)
