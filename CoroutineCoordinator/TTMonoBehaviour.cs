using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extending BaseBehaviour (extending MonoBehaviour) to add some extra functionality. 
/// Merging ideas from::
/// Exception handling & Return Values from: http://twistedoakstudios.com/blog/Post83_coroutines-more-than-you-want-to-know
/// Locking Coroutines to avoid concurrency problems http://www.zingweb.com/blog/2013/02/05/unity-coroutine-wrapper by Tim Tregubov (2013)
/// Solution to pause, unease and kill running coroutines at will https://youtu.be/VnbfEyL85kE by prime[31]
/// </summary>
public class TTMonoBehaviour : BaseBehaviour
{
  private LockQueue LockedCoroutineQueue { get; set; }
          
  /// <summary>
  /// Coroutine with return value AND exception handling on the return value. 
  /// </summary>
  public Coroutine<T> StartCoroutine<T>(IEnumerator coroutine)
  {
      Coroutine<T> coroutineObj = new Coroutine<T>();
      coroutineObj.coroutine = base.StartCoroutine(coroutineObj.InternalRoutine(coroutine));
      return coroutineObj;
  }
  
  /// <summary>
  /// Lockable coroutine. Can either wait for a previous coroutine to finish or a timeout or just bail if previous one isn't done.
  /// Caution: the default timeout is 10 seconds. Coroutines that timeout just drop so if its essential increase this timeout.
  /// Set waitTime to 0 for no wait
  /// </summary>
  public Coroutine<T> StartCoroutine<T>(IEnumerator coroutine, string lockID, float waitTime = 10f)
  {
      if (LockedCoroutineQueue == null) LockedCoroutineQueue = new LockQueue();
      Coroutine<T> coroutineObj = new Coroutine<T>(lockID, waitTime, LockedCoroutineQueue);
      coroutineObj.coroutine = base.StartCoroutine(coroutineObj.InternalRoutine(coroutine));
      return coroutineObj;
  }
  
  /// <summary>
  /// Coroutine with return value AND exception handling AND lockable
  /// </summary>
  public class Coroutine<T>
  {
	private T returnVal;
	private Exception e;
	private string lockID;
	private float waitTime;

	private LockQueue lockedCoroutines; //reference to objects lockdict
	private bool lockable;


	public event System.Action<bool> jobComplete;
	
	private bool _running;
	
	public bool running { get { return _running; } }
	
	private bool _paused;
	
	public bool paused { get { return _paused; } }
	
	private IEnumerator _coroutine;
	private bool _jobWasKilled;
      
	public Coroutine coroutine;
	public T Value
	{
	  get
	  {
	      if (e != null)
	      {
	          throw e;
	      }
	      return returnVal;
	  }
	}

	#region constructors  

      public Coroutine() { lockable = false; }
      public Coroutine(string lockID, float waitTime, LockQueue lockedCoroutines)
      {
          this.lockable = true;
          this.lockID = lockID;
          this.lockedCoroutines = lockedCoroutines;
          this.waitTime = waitTime;
      }

	#endregion

	#region private methods
      
      public IEnumerator InternalRoutine(IEnumerator coroutine)
      {

//				Debug.Log ("InternalRoutine STARTS for: " + coroutine); 

          if (lockable && lockedCoroutines != null)
          {        
              if (lockedCoroutines.Contains(lockID))
              {
                  if (waitTime == 0f)
                  {
                      //Debug.Log(this.GetType().Name + ": coroutine already running and wait not requested so exiting: " + lockID);
                      yield break;
                  } else
                  {
                      //Debug.Log(this.GetType().Name + ": previous coroutine already running waiting max " + waitTime + " for my turn: " + lockID);
                      float starttime = Time.time;
                      float counter = 0f;
                      lockedCoroutines.Add(lockID, coroutine);

                      while (!lockedCoroutines.First(lockID, coroutine) && (Time.time - starttime) < waitTime)
                      {
                          yield return null;
                          counter += Time.deltaTime;
                      }
                      if (counter >= waitTime)
                      {
                          string error = this.GetType().Name + ": coroutine " + lockID + " bailing! due to timeout: " + counter;
                          Debug.LogError(error);
                          this.e = new Exception(error);
                          lockedCoroutines.Remove(lockID, coroutine);
                          yield break;
                      }
                  }
              } else
              {
                  lockedCoroutines.Add(lockID, coroutine);
              }
          }
		
			if (!lockable || lockable && lockedCoroutines != null && lockedCoroutines.First (lockID, coroutine))
				_running = true;
          
			while (_running) 
			{
				if (_paused) 
				{
					yield return null;
				} else {
					try {
						if (!coroutine.MoveNext ()) {
							if (lockable)
								lockedCoroutines.Remove (lockID, coroutine);

								_running = false;
								yield break;
								}
					} catch (Exception e) {
						this.e = e;
						Debug.LogError (this.GetType ().Name + ": caught Coroutine exception! " + e.Message + "\n" + e.StackTrace);
						if (lockable)
								lockedCoroutines.Remove (lockID, coroutine);
						yield break;
					}

					object yielded = coroutine.Current;

					if (yielded != null && yielded.GetType () == typeof(T)) { 	// check each object to see if it's a specific type we care about 														
						returnVal = (T)yielded;								// If it is, we store the type,
						
						if (lockable)
							lockedCoroutines.Remove (lockID, coroutine); // remove coroutine from lockedCoroutines
							
							_running = false;
							yield break;												// & end the routine 
					} else {														// if yielded object is NULL and dont have a specific type we care about
							yield return coroutine.Current;						// return the object
					}
				}
			}
	}
	#endregion


	#region public API

	public void pause ()
	{
		_paused = true;
	}
	
	public void unpause ()
	{
		_paused = false;
	}
	
	public void kill ()
	{
		_jobWasKilled = true;
		_running = false;
		_paused = false;
	}
	
	public void kill (float delayInSeconds)
	{
		var delay = (int)(delayInSeconds * 1000);
		new System.Threading.Timer (obj =>
		                            {
			lock (this) {
				kill ();
			}
		}, null, delay, System.Threading.Timeout.Infinite);
	}
	
	#endregion

  }
  
  
  /// <summary>
  /// coroutine lock and queue
  /// </summary>
  public class LockQueue
  {
      private Dictionary<string, List<IEnumerator>> LockedCoroutines { get; set; }
      

	#region private constructors

      public LockQueue()
      {
          LockedCoroutines = new Dictionary<string, List<IEnumerator>>();
      }

	#endregion
	
	#region public API
      
      /// <summary>
      /// check if LockID is locked
      /// </summary>
      public bool Contains(string lockID)
      {
          return LockedCoroutines.ContainsKey(lockID);
      }
      
      /// <summary>
      /// check if given coroutine is first in the queue
      /// </summary>
      public bool First(string lockID, IEnumerator coroutine)
      {
          bool ret = false;
          if (Contains(lockID))
          {
              if (LockedCoroutines[lockID].Count > 0)
              {
                  ret = LockedCoroutines[lockID][0] == coroutine;
              }
          }
          return ret;
      }
      
      /// <summary>
      /// Add the specified lockID and coroutine to the coroutine lockqueue
      /// </summary>
      public void Add(string lockID, IEnumerator coroutine)
      {
			//Debug.Log ("Added: " + coroutine + " LockID: " + lockID);

          if (!LockedCoroutines.ContainsKey(lockID))
          {
              LockedCoroutines.Add(lockID, new List<IEnumerator>());
          }
          
          if (!LockedCoroutines[lockID].Contains(coroutine))
          {
              LockedCoroutines[lockID].Add(coroutine);
          }
      }
      
      /// <summary>
      /// Remove the specified coroutine and queue if empty
      /// </summary>
      public bool Remove(string lockID, IEnumerator coroutine)
      {

          bool ret = false;
          if (LockedCoroutines.ContainsKey(lockID))
          {
				//Debug.Log ("REMOVED: " + coroutine + " LockID: " + lockID);
              if (LockedCoroutines[lockID].Contains(coroutine))
              {
                  ret = LockedCoroutines[lockID].Remove(coroutine);
              }
              
              if (LockedCoroutines[lockID].Count == 0)
              {
                  ret = LockedCoroutines.Remove(lockID);
              }
          }
          return ret;
      }
	#endregion
      
  }

}


