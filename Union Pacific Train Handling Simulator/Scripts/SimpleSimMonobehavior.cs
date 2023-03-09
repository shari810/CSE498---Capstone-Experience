using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleSimMonobehavior : MonoBehaviour {

	public string TRK_FILE = @"SimulationBuild/Routes/straight100/General Content/World/straight100.trk";
	//public string CST_FILE = @"SimulationBuild\General Content\Trains\15Car Local.cst";
	public string CST_FILE = @"C:\Users\girba\Desktop\Simulation API\Example Application\SimulationBuild\General Content\Trains\15Car Local.cst";
	public double TIMESTEP = 30.0;
	public double START_MILEPOST = 56.0;


	SimpleSimulation.Locomotive.ThrottleNotch currentNotch;
	SimpleSimulation.Consist consist;

	float nextLogTime = 5.0f;
	private Rigidbody2D rb;

	private Vector3 fp;   //First touch position
	private Vector3 lp;   //Last touch position
	private float dragDistance;  //minimum distance for a swipe to be registered

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();

		string buildFolder = System.IO.Path.GetDirectoryName(Application.dataPath);


		SimpleSimulation.Consist.DIESEL_ELECTRICS_DIRECTORY = @"SimulationBuild\General Content\Trains";

		TRK_FILE = System.IO.Path.Combine(buildFolder, TRK_FILE);
		SimpleSimulation.Track track = new SimpleSimulation.Track(TRK_FILE);

		consist = new SimpleSimulation.Consist(CST_FILE);
		track.PlaceConsist(consist, START_MILEPOST);
		List<SimpleSimulation.Locomotive> allLocomotives = consist.GetLocomotives();
		consist.StartSimulating();

		Debug.Log("Car Count: " + consist.GetCarCount());
		Debug.Log("Milepost: " + consist.FirstCar.GetMilepost(true));
		Debug.Log("Velocity: " + consist.FirstCar.GetVelocityMPS());

		allLocomotives[0].SetReverserPosition(SimpleSimulation.Locomotive.ReverserPosition.Forward);



		UpdateThrottleAndSimulate(consist, currentNotch, 30);

	}
	
	// Update is called once per frame
	void Update ()
	{

		if (Input.touchCount == 1) // user is touching the screen with a single touch
		{
			Touch touch = Input.GetTouch(0); // get the touch
			if (touch.phase == TouchPhase.Began) //check for the first touch
			{
				fp = touch.position;
				lp = touch.position;
			}
			else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
			{
				lp = touch.position;
			}
			else if (touch.phase == TouchPhase.Ended) // check if the finger is removed from the screen
			{
				lp = touch.position;  //last touch position. Ommitted if you use list

				//Check if drag distance is greater than 20% of the screen height
				if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
				{//It's a drag
				 //check if the drag is vertical or horizontal
					if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
					{   //If the horizontal movement is greater than the vertical movement...
						if ((lp.x > fp.x))  //If the movement was to the right)
						{   //Right swipe
							Debug.Log("Right Swipe");
							if ((int)currentNotch < 8)
							{
								currentNotch = (SimpleSimulation.Locomotive.ThrottleNotch)((int)currentNotch + 1);
								List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
								foreach (SimpleSimulation.Locomotive loco in allLocos)
								{
									loco.SetThrottleNotch(currentNotch);
								}
							}
						}
						else
						{   //Left swipe
							Debug.Log("Left Swipe");
							if ((int)currentNotch > 0)
							{
								currentNotch = (SimpleSimulation.Locomotive.ThrottleNotch)((int)currentNotch - 1);
								List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
								foreach (SimpleSimulation.Locomotive loco in allLocos)
								{
									loco.SetThrottleNotch(currentNotch);
								}
							}
						}
					}
				}
				else
				{   //It's a tap as the drag distance is less than 20% of the screen height
					Debug.Log("Tap");
				}
			}
		}


		if (Input.GetKeyDown(KeyCode.T))
        {
			if ((int)currentNotch < 8)
			{
				currentNotch = (SimpleSimulation.Locomotive.ThrottleNotch)((int)currentNotch + 1);
				List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
				foreach (SimpleSimulation.Locomotive loco in allLocos)
				{
					loco.SetThrottleNotch(currentNotch);
				}
			}
		}
		else if (Input.GetKeyDown(KeyCode.D))  // replace below with dynamic braking
        {
			if ((int)currentNotch > 0)
			{
				currentNotch = (SimpleSimulation.Locomotive.ThrottleNotch)((int)currentNotch - 1);
				List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
				foreach (SimpleSimulation.Locomotive loco in allLocos)
				{
					loco.SetThrottleNotch(currentNotch);
				}
			}
        }

		consist.Simulate(Time.deltaTime * 1000.0);


		if (Time.time > nextLogTime)
        {
			nextLogTime += 5.0f;
			Debug.Log(string.Format("Time: {2}    Milepost: {0}    Velocity: {1}    Notch: {3}", consist.FirstCar.GetMilepost(true), consist.FirstCar.GetVelocityMPS(), Time.time, currentNotch.ToString()));
		}
	}

    void FixedUpdate()
    {
		rb.velocity = new Vector2((float)consist.FirstCar.GetVelocityMPS(), rb.velocity.y);
	}

    void UpdateThrottleAndSimulate(SimpleSimulation.Consist consist, SimpleSimulation.Locomotive.ThrottleNotch throttleNotch, int numTimesteps)
	{
		List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();


		Debug.Log(string.Format("Throttle: {0}    Milliseconds: {1}", throttleNotch.ToString(), (numTimesteps * TIMESTEP)));
		foreach (SimpleSimulation.Locomotive loco in allLocos)
		{
			loco.SetThrottleNotch(throttleNotch);
		}
		for (int i = 0; i < numTimesteps; i++)
		{
			consist.Simulate(TIMESTEP);
		}
		Debug.Log(string.Format("Milepost: {0}    Velocity: {1}", consist.FirstCar.GetMilepost(true), consist.FirstCar.GetVelocityMPS()));
	}

	void UpdateDbAndSimulate(SimpleSimulation.Consist consist, double dbPercent, int numTimesteps)
	{
		List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();

		Debug.Log(string.Format("DB: {0}    Milliseconds: {1}", dbPercent, (numTimesteps * TIMESTEP)));
		foreach (SimpleSimulation.Locomotive loco in allLocos)
		{
			loco.SetDynamicBrakePercent(dbPercent);
		}
		for (int i = 0; i < numTimesteps; i++)
		{
			consist.Simulate(TIMESTEP);
		}
		Debug.Log(string.Format("Milepost: {0}    Velocity: {1}", consist.FirstCar.GetMilepost(true), consist.FirstCar.GetVelocityMPS()));
	}

	double[] GetInteractionForces(SimpleSimulation.Consist consist)
	{
		double[] forces = new double[consist.GetCarCount() - 1];
		int i = 0;
		for (SimpleSimulation.Car car = consist.FirstCar; car.Next != null; car = car.Next)
		{
			forces[i++] = car.GetInteractionForce();
		}
		return forces;
	}

	double[] GetElevationProfile(SimpleSimulation.Track track, SimpleSimulation.Consist consist, double milesBefore, double milesAfter)
	{
		double consistMilepost = track.GetMilepost(consist, true);
		double beginMilepost = consistMilepost - milesBefore;
		double endMilepost = consistMilepost + milesAfter;

		double[] elevationProfile = new double[500];
		track.GetElevation(beginMilepost, endMilepost, ref elevationProfile);
		return elevationProfile;
	}
}
