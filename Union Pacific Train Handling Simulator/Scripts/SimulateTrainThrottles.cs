using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SimulateTrainThrottles : MonoBehaviour
{

    public string TRK_FILE = @"SimulationBuild/Routes/straight100/General Content/World/straight100.trk";
    //public string CST_FILE = @"SimulationBuild\General Content\Trains\15Car Local.cst";
    public string CST_FILE = @"SimulationBuild\General Content\Trains\Coal - LD(DP)135 Cars LL8081.cst";
    public double TIMESTEP = 30.0;
    public double START_MILEPOST = 56.0;

	private float timestepTimer = 0f;


    List<SimpleSimulation.Locomotive.ThrottleNotch> currentNotches = new List<SimpleSimulation.Locomotive.ThrottleNotch>();
    SimpleSimulation.Consist consist;

    float nextLogTime = 0.25f;
    private Rigidbody2D rb;

	// List of list of locomotive numbers in order from front to back corresponding to locomotive groups
	List<List<int>> locomotiveGroups = new List<List<int>>();

	int locomotiveGroupCount;

	int currentlySelectedLocomotiveGroup = 0;

    // Start is called before the first frame update
    void Start()
    {
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

		foreach (SimpleSimulation.Locomotive loco in allLocomotives)
		{
			loco.SetReverserPosition(SimpleSimulation.Locomotive.ReverserPosition.Forward);
			loco.SetThrottleNotch(SimpleSimulation.Locomotive.ThrottleNotch.Idle);
			loco.SetDynamicBrakePercent(0);
		}

		// Get locomotive groups from children
		int previousCarType = -1;
		int currentCarType;
		int locomotiveNumber = 0;
		foreach (Transform car in transform.Cast<Transform>().OrderBy(t => t.GetSiblingIndex()))
		{
			currentCarType = car.GetComponent<TrainCarInfo>().GetTrainCarType();
			if (currentCarType == 2) // Treat different types of locomotives the same.
				currentCarType = 0;
			// New locomotive group encountered
			if (currentCarType != previousCarType && currentCarType != 1)
			{
				locomotiveGroups.Add(new List<int>());
				locomotiveGroups.Last().Add(locomotiveNumber);
				++locomotiveNumber;
			}
			// Another locomotive within group encountered
			else if (currentCarType != 1)
            {
				locomotiveGroups.Last().Add(locomotiveNumber);
				++locomotiveNumber;
            }
			previousCarType = currentCarType;
		}

		locomotiveGroupCount = locomotiveGroups.Count;

		for (int i = 0; i < locomotiveGroupCount; ++i)
        {
			currentNotches.Add(0);
        }

		//for(int i = 0; i < locomotiveGroups.Count; ++i)
  //      {
		//	foreach (var locomotiveNum in locomotiveGroups[i])
  //          {
		//		Debug.Log(i.ToString() + " " + locomotiveNum.ToString());
  //          }
  //      }

		//UpdateThrottleAndSimulate(consist, currentNotches[currentlySelectedLocomotiveGroup], 30);
	}

    // Update is called once per frame
    void Update()
    {
		//Debug.Log(currentlySelectedLocomotiveGroup);
		// Select right locomotive group (default is front--rightmost)
		if (Input.GetKeyDown(KeyCode.D))
        {
			if (currentlySelectedLocomotiveGroup > 0)
				--currentlySelectedLocomotiveGroup;
			Debug.Log("Current throttle on " + currentlySelectedLocomotiveGroup + ": " + currentNotches[currentlySelectedLocomotiveGroup]);
		}
		// Select left locomotive group (default is front--rightmost)
		else if (Input.GetKeyDown(KeyCode.A))
        {
			if (currentlySelectedLocomotiveGroup < locomotiveGroupCount - 1)
				++currentlySelectedLocomotiveGroup;
			Debug.Log("Current throttle on " + currentlySelectedLocomotiveGroup + ": " + currentNotches[currentlySelectedLocomotiveGroup]);
		}
		else if (Input.GetKeyDown(KeyCode.T))
		{
			if ((int)currentNotches[currentlySelectedLocomotiveGroup] < 8)
			{
				currentNotches[currentlySelectedLocomotiveGroup] = (SimpleSimulation.Locomotive.ThrottleNotch)((int)currentNotches[currentlySelectedLocomotiveGroup] + 1);
				List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
				// Idle state
				if ((int)currentNotches[currentlySelectedLocomotiveGroup] == 0)
                {
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{
						
						allLocos[locoIndex].SetThrottleNotch(currentNotches[currentlySelectedLocomotiveGroup]);
					}
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{
						
						allLocos[locoIndex].SetDynamicBrakePercent(0);
					}
				}
				// Positive throttle
				else if ((int)currentNotches[currentlySelectedLocomotiveGroup] > 0)
                {
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{
						
						allLocos[locoIndex].SetThrottleNotch(currentNotches[currentlySelectedLocomotiveGroup]);
					}
				}
				// Dynamic braking
				else
                {
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{
						
						allLocos[locoIndex].SetDynamicBrakePercent(((int)currentNotches[currentlySelectedLocomotiveGroup])*-12.5);
					}
				}
			}
			Debug.Log("Current throttle on " + currentlySelectedLocomotiveGroup + ": " + currentNotches[currentlySelectedLocomotiveGroup]);
		}
		else if (Input.GetKeyDown(KeyCode.B)) 
		{
			if ((int)currentNotches[currentlySelectedLocomotiveGroup] > -8)
			{
				currentNotches[currentlySelectedLocomotiveGroup] = (SimpleSimulation.Locomotive.ThrottleNotch)((int)currentNotches[currentlySelectedLocomotiveGroup] - 1);
				List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
				// Idle state
				if ((int)currentNotches[currentlySelectedLocomotiveGroup] == 0)
				{
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{
						
						allLocos[locoIndex].SetThrottleNotch(currentNotches[currentlySelectedLocomotiveGroup]);
					}
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{
						
						allLocos[locoIndex].SetDynamicBrakePercent(0);
					}
				}
				// Positive throttle
				else if ((int)currentNotches[currentlySelectedLocomotiveGroup] > 0)
				{
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{
						
						allLocos[locoIndex].SetThrottleNotch(currentNotches[currentlySelectedLocomotiveGroup]);
					}
				}
				// Dynamic braking
				else
				{
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{
						
						allLocos[locoIndex].SetDynamicBrakePercent(((int)currentNotches[currentlySelectedLocomotiveGroup]) * -12.5);
					}
				}
			}
			Debug.Log("Current throttle on " + currentlySelectedLocomotiveGroup + ": " + currentNotches[currentlySelectedLocomotiveGroup]);
		}

		if (timestepTimer*1000.0 >= TIMESTEP)
        {
			consist.Simulate(TIMESTEP);
			timestepTimer = 0f;
		}

		timestepTimer += Time.deltaTime;

        // Why does trying to get the throttle notch throw an error? Could this be a bug with the Physics API?
        //if (Time.time > nextLogTime)
        //{
        //	nextLogTime += 0.25f;
        //	List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
        //	for (int i = 0; i < allLocos.Count; ++i )
        //          {
        //		Debug.Log(string.Format("Locomotive {0}'s Throttle: {1}", i, allLocos[i].GetThrottleNotch().ToString()));
        //          }
        //}
        if (Time.time > nextLogTime)
        {
			//Debug.Log(string.Format("Milepost: {0}    Velocity: {1}", consist.FirstCar.GetMilepost(true), consist.FirstCar.GetVelocityMPS()));
		}

		// Set all velocities and forces
		double[] interactionForces = GetInteractionForces(consist);
		//Debug.Log(interactionForces.Length);
		int index = 0;
		foreach (Transform car in transform.Cast<Transform>().OrderBy(t => t.GetSiblingIndex()))
        {
			//car.GetComponent<SmoothTrainMovement>().movementSpeed = (float)consist.FirstCar.GetVelocityMPS();
			// Exclude last car (see description of interaction forces)
			if (index < consist.GetCarCount()-1)
				car.GetComponent<ColorGradient>().forces = (float)interactionForces[index];
			++index;
        }


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
