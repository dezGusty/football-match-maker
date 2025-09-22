import { Injectable } from '@angular/core';
import { initializeApp, FirebaseApp } from 'firebase/app';
import {
  getFirestore,
  Firestore,
  collection,
  getDocs,
} from 'firebase/firestore';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class FirebaseService {
  private app: FirebaseApp;
  private firestore: Firestore;

  constructor() {
    this.app = initializeApp(environment.firebaseConfig);
    this.firestore = getFirestore(this.app);
  }

  async getAllMatches(): Promise<any[]> {
    try {
      const querySnapshot = await getDocs(
        collection(this.firestore, 'matches')
      );
      const matches: any[] = [];

      querySnapshot.forEach((doc) => {
        matches.push({ id: doc.id, ...doc.data() });
      });

      return matches;
    } catch (error) {
      console.error('Error getting matches:', error);
      throw error;
    }
  }

  async getAllRatings(): Promise<any[]> {
    try {
      const querySnapshot = await getDocs(
        collection(this.firestore, 'ratings')
      );
      const ratings: any[] = [];

      querySnapshot.forEach((doc) => {
        ratings.push({ id: doc.id, ...doc.data() });
      });

      return ratings;
    } catch (error) {
      console.error('Error getting ratings:', error);
      throw error;
    }
  }
}
