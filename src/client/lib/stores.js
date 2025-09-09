import { writable } from 'svelte/store';



// Events store
export const events = writable([]);

// Categories store
export const categories = writable([]);


export const addCategory = (Addcategories) => {
	categories.update(currentCategories => [...currentCategories, Addcategories]);
};

export const updateCategory = (id, updateCategories) => {
	categories.update(currentCategories =>
		currentCategories.map(category =>
			category.id === id ? { ...category, ...updatedCategories } : category
		)
	)
}

export const deleteCategory = (id) => {
	categories.update(currentCategories =>
		currentCategories.filter(category => category.id !== id)
	)
}

export const setCategories = (categoriesList) => {
	categories.set(categoriesList);
}