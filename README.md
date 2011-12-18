Warning
=======
This is experimental code that will need to be modified to work for you. I have not gotten this to a point of dynamic configuration.

Requirements
============
* pq is backed by MongoDB so you will need the MongoDB drivers found at [here](http://www.mongodb.org/display/DOCS/CSharp+Language+Center)
* the pq app is run on ASP.NET 4

Process
=======
The pq app allows users to register, login, and upload photos but that's not really the point of the program it's more of a test. The app enters the images uploaded into the Mongo database and stores them in a particular directory named by their ObjectId given from the mongo entry.

The server runs on a separate port and waits for http requests.requests should be made to the root of the server address, so there shouldn't be any path. Parameters should be given as a query string. An 'id' parameter must always be given and it should be the ObjectId of the image you want. This can be either a base image and have more parameters, or it can be any child image. When you pass parameters to manipulate the image, it will create a new image and save it. You can get this image by requesting it's ID, or it's parents ID with the same parameters that were given to create it.

Currently Available Parameters
==============================

    id - this must always be given

* optional set 1 - Resizing
	
	width - the desired width of the resized image
	height - the desired height of the resized image

* optional set 2 - Cropping
	
	cropx1 - the x coordinate for top left corner of the cropping rectangle
	cropy1 - the y coordinate for the top left
	cropx2 - the x coordinate for the bottom right corner of the cropping rectangle
	cropy2 - the y coordinate for the bottom right


Misc
====
Note that when passing both rezsize and crop parameters, cropping will take place first.
