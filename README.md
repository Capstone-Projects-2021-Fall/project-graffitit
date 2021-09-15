# GraffitIT

## Project Overview

GraffitIT is a content sharing mobile application that stores photos, videos, and sounds at particular locations using tools such as the Google Maps API, SQL, and AWS S3. GraffitIT will capture visual media or audio from the user’s phone’s built-in camera and use Unity 3D as its primary framework, which will allow for AR capabilities and compatibility with both Android and Apple mobile devices. 

GraffitIT encourages an interactive experience while traveling to new locations. Since the media is only displayed when a user is within the same location that it was uploaded to, this leads to the experience of actively discovering content rather than simply browsing through content.

The application begins with an AR camera viewing mode. The user can then capture content with their device, and tag it as a photo, video, or sound. The user created content is now uploaded and associated with that location, using the Google Maps API to tag geolocation data to their content. When the user enters camera viewing mode again at that location, they will be able to see other tagged content from other users using the AR feature from Unity 3D. 

Alternatively, the user can view other users’ shared media. Based on which direction they’re facing, the user could potentially see several points of interest in one geographic location. 

AWS is a serverless solution that will be storing and retrieving arbitrary data using a key. That key will be stored using SQL, along with user data. 


## Contributors
Noah Williams <br>
Hadeel Alebbi <br>
Soobin Kim <br>
Mario Libohova <br>
Peter Marion <br>
Wayne Zheng <br>
