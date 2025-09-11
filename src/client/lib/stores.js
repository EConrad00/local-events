import { writable } from 'svelte/store';



// Events store
export const events = writable([]);

export const addEvent = (addEvents) => {
  events.update(currentEvents => [...currentEvents, addEvents]);
};


export const setEvents = (eventsList) => {
	events.set(eventsList);
};

export const deleteEvent = (id) => {
	events.update(currentEvents=>
		currentEvents.filter(event=> event.id !==id)
	);
};
export const updateEvent = (id, updatedEvents) => {
	events.update(currentEvents =>
		currentEvents.map(event=>
			event.id === id ? {...event, ...updatedEvents} : event
		) 
	);
};

// Categories store
export const categories = writable([]);


export const addCategory = (Addcategories) => {
	categories.update(currentCategories => [...currentCategories, Addcategories]);
};

export const updateCategory = (id, updatedCategories) => {
	categories.update(currentCategories =>
		currentCategories.map(category =>
			category.id === id ? { ...category, ...updatedCategories } : category
		)
	);
};

export const deleteCategory = (id) => {
	categories.update(currentCategories =>
		currentCategories.filter(category => category.id !== id)
	);
};

export const setCategories = (categoriesList) => {
	categories.set(categoriesList);
};

