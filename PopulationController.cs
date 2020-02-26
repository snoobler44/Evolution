using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationController : MonoBehaviour {
	List<GeneticPathFinder> population = new List<GeneticPathFinder>();
	public GameObject creaturePrefab;
	public int populationSize = 100;
	public int genomeLength;
	public int survivorKeep = 5;
	int generation;
	public float cutoff = 0.3f;
	[Range(0f, 1)]
	public float mutationRate = 0.01f;
	public Transform spawnPoint;
	public Transform end;

	void InitPopulation() {
		// for each creature, slap it into a population and add GeneticPathFinder();
		for(int i = 0; i < populationSize; i++) {
			GameObject go = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity);
			go.GetComponent<GeneticPathFinder>().InitCreature(new DNA(genomeLength), end.position);
			population.Add(go.GetComponent<GeneticPathFinder>());
		}
	}

	void NextGeneration() {
		int survivorCut = Mathf.RoundToInt(populationSize * cutoff);
		List<GeneticPathFinder> survivors = new List<GeneticPathFinder> ();

		for (int i = 0; i < survivorCut; i++) {
			survivors.Add(GetFittest());
		}
		for (int i = 0; i < population.Count; i++) {
			Destroy(population[i].gameObject);
		}
		population.Clear();

		// top 5 survivors pass their genes
		for (int i = 0; i < survivorKeep; i++) {
			GameObject go = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity);
			go.GetComponent<GeneticPathFinder>().InitCreature(survivors[i].dna, end.position);
			population.Add(go.GetComponent<GeneticPathFinder>());
		}
		while (population.Count < populationSize) {
			for (int i = 0; i < survivors.Count; i++) {
				GameObject go = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity);
				go.GetComponent<GeneticPathFinder>().InitCreature(new DNA(survivors[i].dna, survivors[Random.Range(0,10)].dna, mutationRate), end.position);
				population.Add(go.GetComponent<GeneticPathFinder>());
				if (population.Count >= populationSize) {
					break;
				}
			}
		}
		for (int i = 0; i < survivors.Count; i++) {
			Destroy(survivors[i].gameObject);
		}
	}

	private void Start() {
		InitPopulation();
	}

	private void Update() {
		if (!HasActive()) {
			NextGeneration();
			generation++;
		}
	}

	void OnGUI() {
		GUI.Label (new Rect (20, 20, 200, 40), "Geneartion: " + generation);
	}

	GeneticPathFinder GetFittest() {
		float maxFitness = float.MinValue;
		int index = 0;
		for (int i = 0; i < population.Count; i++) {
			if (population[i].fitness > maxFitness) {
				maxFitness = population [i].fitness;
				index = i;
			}
		}
		GeneticPathFinder fittest = population [index];
		population.Remove(fittest);
		return fittest;
	}

	bool HasActive() {
		for (int i = 0; i < population.Count; i++) {
			if (!population[i].hasFinished) {
				return true;
			}
		}
		return false;
	}
}
