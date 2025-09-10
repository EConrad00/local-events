<script>
    import { onMount } from 'svelte';
    import {writable} from 'svelte/store';
    import {categories, addCategory, deleteCategory, updateCategory, setCategories} from '$lib/stores'

	let events = [];
	//let categories = [];
	// Form data for new event
	let eventName = '';
	let eventDescription = '';
	let eventDateTime = '';
	let eventLocation = '';
	let selectedCategoryIds = [];
	
    let categoryName = '';
    let newCategoryName ='';
    let editingCategory = null;
    let editingCategoryName = '';
    let selectedCategoriesForDelete = [];
	
	function formatLocalDateTime(dateTimeString) {
        if(!dateTimeString) return 'TBD';
        const date = new Date(dateTimeString);
        return date.toLocaleString(); 
    }

	// Function to submit new event
	async function submitEvent() {
		if (!eventName || !eventDateTime || !eventLocation) {
			alert('Please fill in all fields');
			return;
		}

		try {
			const response = await fetch('/api/events', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({
					name: eventName,
					description: eventDescription,
					dateTime: new Date(eventDateTime).toISOString(),
					location: eventLocation,
					CategoryId: selectedCategoryIds.map(id => parseInt(id))
				})
			});

			if (response.ok) {
				// Clear form
				eventName = '';
				eventDescription = '';
				eventDateTime = '';
				eventLocation = '';
				selectedCategoryIds = [];

				// Refresh events list 
				const eventsResponse = await fetch('/api/events');
				if (eventsResponse.ok) {
					events = await eventsResponse.json();
				}
				alert('Event created successfully!');
			} else {
				const error = await response.text();
				alert('Error creating event: ' + error);
			}
		} catch (err) {
			console.error('Error creating event:', err);
			alert('Error creating event: ' + err.message);
		}
	}

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
        // Fetch events
        try {
            const eventsResponse = await fetch('/api/events');
            if (eventsResponse.ok) {
                events = await eventsResponse.json();
            }
        } catch (err) {
            console.error('Error fetching events:', err);
        }

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

<h1>Local Events</h1>
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
                <input 
                type="checkbox"
                checked={editingCategory === category.id}
                on:change={(e) => editingCategory = e.target.checked ? category.id : null}
                >
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
    <div>
        <h2>Events</h2>
        {#if events.length > 0}
            <ul>
                {#each events as event}
                    <li>
                        <strong>{event.name}</strong> - {event.description}
                        <br>
                        <small>Date: {formatLocalDateTime(event.dateTime)} | Location: {event.location || 'TBD'}</small>
                    </li>
                {/each}
            </ul>
        {:else}
            <p>No events found.</p>
        {/if}
        
        <h2>Add New Event</h2>
        <form on:submit|preventDefault={submitEvent}>
            <div class="right">
                <label for="eventName">Event Name:</label>
                <input 
                    type="text" 
                    id="eventName" 
                    bind:value={eventName} 
                    required 
                    placeholder="Enter event name"
                >
            </div>
            
            <div>
                <label for="eventDescription">Description:</label>
                <textarea 
                    id="eventDescription" 
                    bind:value={eventDescription} 
                    placeholder="Enter event description"
                ></textarea>
            </div>
            
            <div>
                <label for="eventDateTime">Date & Time:</label>
                <input 
                    type="datetime-local" 
                    id="eventDateTime" 
                    bind:value={eventDateTime} 
                    required
                >
            </div>
            
            <div>
                <label for="Addcategories"> Categories (optional):</label>
                <div class="checkbox-group">
                    {#each $categories as category}
                        <label class="checkbox-label">
                            <input 
                                type="checkbox" 
                                value={category.id}
                                bind:group={selectedCategoryIds}
                            >
                            {category.name}
                        </label>
                    {/each}
                </div>
            </div>
            
            <div>
                <label for="eventLocation">Location:</label>
                <input 
                    type="text" 
                    id="eventLocation" 
                    bind:value={eventLocation} 
                    required 
                    placeholder="Enter event location"
                >
            </div>
            
            <button type="submit">Create Event</button>
        </form>
    </div>
</div>
<style>
    
    .container{
        display: flex;
        gap: 200px;
    }
    .left{
        width: 250px;
    }

    form {
        max-width: 500px;
        margin: 20px 0;
    }
    
    form div {
        margin-bottom: 10px;
    }
    
    label {
        display: block;
        margin-bottom: 5px;
        font-weight: bold;
    }
    
    input, textarea {
        width: 100%;
        padding: 8px;
        border: 1px solid #ccc;
        border-radius: 4px;
        font-size: 14px;
    }
    
    textarea {
        height: 80px;
        resize: vertical;
    }
    
    button {
        background-color: #007bff;
        color: white;
        padding: 10px 20px;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        font-size: 16px;
    }
    
    button:hover {
        background-color: #0056b3;
    }
    
    .delete-button {
        background-color: #e40505;
        color: white;
        padding: 10px 20px;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        font-size: 12px;
        margin-top: 10px;
        transition: background-color 0.2s ease;
    }

    
    .delete-button:hover {
        background-color: #9a0000;
    }

    /* hr {
        margin: 30px 0;
        border: none;
        border-top: 1px solid #ccc;
    } */
    
    .checkbox-group {
        display: flex;
        flex-direction: column;
        gap: 8px;
        padding: 8px;
        border: 1px solid #ccc;
        border-radius: 4px;
        background-color: #f9f9f9;
    }
    
    .checkbox-label {
        display: flex;
        align-items: center;
        font-weight: normal;
        margin-bottom: 0;
        cursor: pointer;
    }
    
    .checkbox-label input[type="checkbox"] {
        width: auto;
        margin-right: 8px;
        margin-bottom: 0;
    }
</style>
