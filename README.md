# Poker

This is a monorepo containing C# projects for a mobile app along with 2 web servers.
The first web server is the SignalR hub.  This facilitates the real-time connection between mulitple instances of the mobile app.
The other web server is for storing leaderboard information, and was created with Swagger.

To see a quick demonstration of how all of these things work together (the final product), download and view the PokerDemo.mp4 video.

---

If you instead want to run the app on your own (not recommended, may require additional setup):

0. Setup environment

	1. Install Visual Studio along with the 'ASP.NET for web development' and 'Mobile development with .NET' workloads
	
	2. Clone this repository and open in Visual Studio
	
	3. Build solution and install dependencies

1. Create 2 separate android emulators and download the app on each
	- to download the app, you can just run the Poker.ANDROID project with the emulator as the target
	- I used the default 'Pixel 5 -API 30' emulators, but it should also work with other versions.

2. Run the SignalR hub ('SignalR' project):
	- ensure that the URL is 127.0.0.1:5000

3. Run the web service ('WebAPI' project):
	- ensure that the URL is 127.0.0.1:51905/swagger/index.html

4. Open the app on each emulator and play!
