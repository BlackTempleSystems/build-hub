import { Injectable } from "@angular/core";

/**
 * Local storage wrapper service 
 */
@Injectable({
    providedIn: 'root'
})
export class LocalStorageService {
    constructor() {
    }

    /**
     * Sets and item in the local storage by given key
     * @param key
     * @param value 
     */
    setItem<T>(key: string, value: T) {
        localStorage.setItem(key, JSON.stringify(value));
    }

    /**
     * Gets an item by a key
     * @param key 
     * @returns 
     */
    getItem<T>(key: string): T {
        return localStorage.getItem(key) as T;
    }

    /**
     * Removes an item by a given key
     * @param key 
     */
    removeItem(key: string) {
        localStorage.removeItem(key);
    }

    /**
     * Clears the local storage.
     */
    clear(): void {
        localStorage.clear();
    }
}