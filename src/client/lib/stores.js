import { writable } from 'svelte/store';



// Events store
export const events = writable([]);

// Categories store
export const categories1 = writable([]);


export const addCategory = (categories1) => {
	categories1.update(currentCategories => [...currentCategories, categories1]);
};

export const updateCategory = (id,updateCategory) => {
	categories1.update(currentCategories =>
		currentCategories.map(category =>
			category.id === id ? { ...category, ...updatedCategory } : category
		)
	)
}

export const deleteCategory = (id) => {
	categories1.update(currenmtCategories =>
		currentCategories.filter(category => category.id !== id)
	)
}

export const setCategories = (categoriesList) => {
	categories1.set(categoriesList);
}