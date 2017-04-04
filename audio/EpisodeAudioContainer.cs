using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EpisodeAudioClipContainer : Singleton<EpisodeAudioClipContainer>  {

	[System.Serializable]
	public class StartupPool
	{
		public List<AudioClip> ambiance = new List<AudioClip>();
		
		public List<AudioClip> ldSc = new List<AudioClip>();
		
		public List<AudioClip> score = new List<AudioClip>();

		public List<AudioClip> Chords = new List<AudioClip>();
		public List<AudioClip> Events = new List<AudioClip>();

		public List<AudioClip> supplChar = new List<AudioClip>();
		public List<AudioClip> supplChar001_ = new List<AudioClip>();
		public List<AudioClip> supplChar001_ani1 = new List<AudioClip>();
		public List<AudioClip> supplChar001_ani2 = new List<AudioClip>();
		public List<AudioClip> supplChar002_ = new List<AudioClip>();
		public List<AudioClip> supplChar002_ani1 = new List<AudioClip>();
		public List<AudioClip> supplChar002_ani2 = new List<AudioClip>();
	}



	public StartupPool PapaGyerek = new StartupPool();

	public StartupPool Szerelo = new StartupPool();
		
	public StartupPool Undefined = new StartupPool();


	private int stringSectionLength = 4;				// determines the lenght of information sections in prefab's name to unfold
	private int sequenceNumber = 0;						// determines the number of the sequence refering to the relevant section of prefab's name


	public void Starter ()
	{
		string epi;
		string epi1;
		string epi2;


		foreach (AudioClip o in Resources.LoadAll("ChordEvent/Chords",typeof(AudioClip))) {
			
			epi = o.chipChopi(stringSectionLength, sequenceNumber);
			
			if (epi == "008_")
			{
				PapaGyerek.Chords.Add(o);					
			}
			
			else if (epi == "013_")
			{
				Szerelo.Chords.Add(o);					
			} 
			
			else if (epi == "000_")
			{
				Undefined.Chords.Add(o);					
			} 
			
			else{}
			
		}

		foreach (AudioClip o in Resources.LoadAll("ChordEvent/Events",typeof(AudioClip))) {
			
			epi = o.chipChopi(stringSectionLength, sequenceNumber);
			
			if (epi == "008_")
			{
				PapaGyerek.Events.Add(o);					
			}
			
			else if (epi == "013_")
			{
				Szerelo.Events.Add(o);					
			} 
			
			else if (epi == "000_")
			{
				Undefined.Events.Add(o);					
			} 
			
			else{}
			
		}

		foreach (AudioClip o in Resources.LoadAll("ambiance",typeof(AudioClip))) {

				epi = o.chipChopi(stringSectionLength, sequenceNumber);

				if (epi == "008_")
				{
					PapaGyerek.ambiance.Add(o);					
				}

				else if (epi == "013_")
				{
					Szerelo.ambiance.Add(o);					
				} 

				else if (epi == "000_")
				{
					Undefined.ambiance.Add(o);					
				} 

				else{}

		}

		foreach (AudioClip o in Resources.LoadAll("ldSc",typeof(AudioClip))) {
			
				epi = o.chipChopi(stringSectionLength, sequenceNumber);
				
				if (epi == "008_")
				{
					PapaGyerek.ldSc.Add(o);					
				}
				
				else if (epi == "013_")
				{
					Szerelo.ldSc.Add(o);					
				} 

				else if (epi == "000_")
				{
					Undefined.ldSc.Add(o);					
				} 
				
				else{}
		}

		foreach (AudioClip o in Resources.LoadAll("score",typeof(AudioClip))) {
			
				epi = o.chipChopi(stringSectionLength, sequenceNumber);
				
				if (epi == "008_")
				{
					PapaGyerek.score.Add(o);					
				}
				
				else if (epi == "013_")
				{
					Szerelo.score.Add(o);					
				} 

				else if (epi == "000_")
				{
					Undefined.score.Add(o);					
				} 
				
				else{}			
		}



		foreach (AudioClip o in Resources.LoadAll("supplChar",typeof(AudioClip))) {

				epi = o.chipChopi(stringSectionLength, 0);
				epi1 = o.chipChopi(stringSectionLength, 1);
				epi2 = o.chipChopi(stringSectionLength, 2);
				////////////
				if (epi == "008_")
				{
					if (epi1 == "001_")
					{
						if (epi2 == "ani1")
						{
						PapaGyerek.supplChar001_ani1.Add(o);
						}
						else if (epi2 == "ani2")
						{
							PapaGyerek.supplChar001_ani2.Add(o);
						} else {
							PapaGyerek.supplChar001_.Add(o);					
						}
					}
					else if (epi1 == "002_")
					{
						if (epi2 == "ani1")
						{
							PapaGyerek.supplChar002_ani1.Add(o);
						}
						else if (epi2 == "ani2")
						{
							PapaGyerek.supplChar002_ani2.Add(o);
						} else {
							PapaGyerek.supplChar002_.Add(o);					
						}
					}
					
					else 
						PapaGyerek.supplChar.Add(o);	

				}
				////////////
				else if (epi == "013_")
				{
					
				if (epi1 == "001_")
				{
					if (epi2 == "ani1")
					{
						Szerelo.supplChar001_ani1.Add(o);
					}
					else if (epi2 == "ani2")
					{
						Szerelo.supplChar001_ani2.Add(o);
					} else {
						Szerelo.supplChar001_.Add(o);					
					}
				}
				else if (epi1 == "002_")
				{
					if (epi2 == "ani1")
					{
						Szerelo.supplChar002_ani1.Add(o);
					}
					else if (epi2 == "ani2")
					{
						Szerelo.supplChar002_ani2.Add(o);
					} else {
						Szerelo.supplChar002_.Add(o);					
					}
				}
						Szerelo.supplChar.Add(o);	
							
				} 

				else if (epi == "000_")
				{
//					epi = o.chipChopi(stringSectionLength, sequenceNumber);
//					epi1 = o.chipChopi(stringSectionLength, 1);
					
					if (epi1 == "001_")
						Undefined.supplChar001_.Add(o);					
					
					else if (epi1 == "002_")
						Undefined.supplChar002_.Add(o);				
					
					else 
						Undefined.supplChar.Add(o);	
					
							
				} 
				
				else{}							
		}

	}







}
