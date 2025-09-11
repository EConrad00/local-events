<script>
    import { onMount } from 'svelte';   
    import {writable} from 'svelte/store';
    import {categories, addCategory, deleteCategory, updateCategory, setCategories } from '$lib/stores'
    import '../../app.css'; // from routes/ to app.css

    let categoryName = '';
    let newCategoryName ='';
    let editingCategory = null;
    let editingCategoryName = '';
    let selectedCategoriesForDelete = [];
    
      async function createCategory() {
        
        const response = await fetch('/api/categories', { 
                method: 'POST',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({
                    name : categoryName
                })
        });
        const newCategory = await response.json();

        addCategory(newCategory)
        categoryName = "";

        //alert("Category created")
    }

    async function deleteCategories() {
        for(const categoryId of selectedCategoriesForDelete){
            const response = await fetch(`/api/categories/${categoryId}`, { 
                method: 'DELETE'
        });
            if(response.ok){

                deleteCategory(parseInt(categoryId));
            }
        }
        selectedCategoriesForDelete = [];
    }

    async function alterCategory() {
        try {
            const categoryId = editingCategory;
            const response = await fetch(`/api/categories/${categoryId}`, {
                method : 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    name : editingCategoryName
                })
            });

            if(response.ok){

                const alteredCategory = await response.json();
        
                updateCategory(parseInt(alteredCategory.id), {name : alteredCategory.name})
                editingCategoryName = "";
                editingCategory = null;
            }
        } catch (err) {
            console.error('Error fetching category')
            alert(err.message);
        }
    }

        onMount(async () => {

        // Fetch categories
        try {
            const categoriesResponse = await fetch('/api/categories');
            if (categoriesResponse.ok) {
                const categoriesData = await categoriesResponse.json();
                setCategories(categoriesData);
            }
        } catch (err) {
            console.error('Error fetching categories:', err);
        }
    });


</script>

<h1>Local categories</h1>
<div class="container">

    <div class="left">
        <h2>Categories</h2>
        {#if $categories.length > 0}
        <div>
            {#each $categories as category}
            <div class="checkbox-label">
                <input
                    type="checkbox"
                    value={category.id}
                    bind:group={selectedCategoriesForDelete}
                >
                {category.name}
                <button
                    type="button"
                    class="edit-pen {editingCategory === category.id ? 'active' : ''}"
                    on:click={() => {
                        if(editingCategory === category.id){
                            editingCategory = null;
                            editingCategoryName = '';
                        } 
                        else{
                            editingCategory = category.id;
                            editingCategoryName = category.name;
                        }
                    }}  
                    >
                    <!-- editingCategory = editingCategory === category.id ? null : category.id -->
                    ✏️
            </button>
            </div>
            {/each}
        </div>
            {#if selectedCategoriesForDelete.length > 0}
                <button
                    type="button"
                    on:click={deleteCategories}
                    class="delete-button"
                    >
                    Delete Selected ({selectedCategoriesForDelete.length})
                </button>
            {/if}
            {#if editingCategory > 0}
                <form on:submit|preventDefault={alterCategory}>
                        <input 
                        type="text"  
                        bind:value={editingCategoryName} 
                        required 
                        placeholder="Enter New Name"
                        >
                        <button class="btn" type="submit" style="margin-top: 10px;">Update Category</button>
                    </form>
            {/if}
        {:else}
        <p>No categories found.</p>
        {/if}
        
        <!-- <hr> -->
        
        <h2>Add New Category</h2>
        <form on:submit|preventDefault={createCategory}>
            <div>
                <label for="categoryName">Category Name:</label>
                <input 
                type="text" 
                id="categoryName" 
                bind:value={categoryName} 
                required 
                placeholder="Enter category name"
                >
                </div>
                <button type="submit">Create Category</button>
            </form>
        
    </div>
</div>