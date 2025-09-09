<script>
    import { onMount } from 'svelte';

	let events = [];
	let categories = [];

	// Form data for new event
	let eventName = '';
	let eventDescription = '';
	let eventDateTime = '';
	let eventLocation = '';
	let selectedCategoryIds = [];
	let eventCategory = '';



	
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
                categories = await categoriesResponse.json();
            }
        } catch (err) {
            console.error('Error fetching categories:', err);
        }
    });
</script>

<h1>Local Events</h1>

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

<h2>Categories</h2>
{#if categories.length > 0}
    <ul>
        {#each categories as category}
            <li>{category.name}</li>
        {/each}
    </ul>
{:else}
    <p>No categories found.</p>
{/if}

<hr>

<h2>Add New Event</h2>
<form on:submit|preventDefault={submitEvent}>
    <div>
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
            {#each categories as category}
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

<style>
    form {
        max-width: 500px;
        margin: 20px 0;
    }
    
    form div {
        margin-bottom: 15px;
    }
    
    label {
        display: block;
        margin-bottom: 5px;
        font-weight: bold;
    }
    
    input, textarea, select {
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
    
    hr {
        margin: 30px 0;
        border: none;
        border-top: 1px solid #ccc;
    }
    
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
