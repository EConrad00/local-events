
<script>
    import { onMount } from 'svelte';
    import {writable} from 'svelte/store';
    import {categories, setCategories, events, setEvents, addEvent, deleteEvent, updateEvent } from '$lib/stores'
    import '../../app.css'; 


	// Form data for new event
	let eventName = '';
	let eventDescription = '';
	let eventDateTime = '';
	let eventLocation = '';
    let eventId = null;
	let selectedCategoryIds = [];
    let editingEvent = null;
    let editingEventName = null;
    let editingEventDescription = null;
    let editingEventDateTime = null;
    let editingEventLocation = null;
    let editingEventCategoryIds = [];
	
	
	function formatLocalDateTime(dateTimeString) {
        if(!dateTimeString) return 'TBD';
        const date = new Date(dateTimeString);
        return date.toLocaleString(); 
    }

	//Function to submit new event
	async function submitEvent() {

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
				
				const newEvents = await response.json();

                addEvent(newEvents)

				eventName = '';
				eventDescription = '';
				eventDateTime = '';
				eventLocation = '';
				selectedCategoryIds = [];
				
			} 

		} catch (err) {
			console.error('Error creating event:', err);
			alert('Error creating event: ' + err.message);
		}
	}
    async function deleteEvents() {
        const response =await fetch (`/api/events/${eventId}`, { 
                method: 'DELETE' 
            
            });
             if(response.ok){

                deleteEvent(parseInt(eventId));
                
            }
            eventId =null;

    }
    async function alterEvents(){
            const eventIds = editingEvent;
            const response =await fetch (`/api/events/${eventIds}`, { 
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
					name: editingEventName,
					description: editingEventDescription,
					dateTime: new Date(editingEventDateTime),
					location: editingEventLocation,
					CategoryId: editingEventCategoryIds.map(id => parseInt(id))
				})
            
            });
             if(response.ok){

                const alteredEvent = await response.json();
        
                updateEvent(parseInt(alteredEvent.id), {
                    name : alteredEvent.name, 
                    description : alteredEvent.description,
                    dateTime : alteredEvent.dateTime, 
                    location : alteredEvent.location, 
                    categories : alteredEvent.categories || []
                })
                editingEventName = "";
                //editingEvent = null;
            }
            editingEvent =null;
    }
    
    

    onMount(async () => {
        // Fetch events
        try {
            const eventsResponse = await fetch('/api/events');
            if (eventsResponse.ok) {
                const eventsData = await eventsResponse.json();
                setEvents(eventsData);
            }
        } catch (err) {
            console.error('Error fetching events:', err);
        }

       
    });
</script>

<h1>Events</h1>

<div class="container">
   
    <div>
        <h2>Current events</h2>
        {#if $events.length > 0}
        <div class="div-scroll">
            {#each $events as event}
            <div class="checkbox-label">
                <input 
                    type="checkbox"
                    checked={eventId === event.id}
                     on:change={(e) => eventId = e.target.checked ? event.id : null}
                >
                <div>
                <strong>Event: {event.name} </strong>
                <div class="div-desc"> 
                Description - {event.description}
                </div>
                <small> Date: {formatLocalDateTime(event.dateTime)} | Location: {event.location || 'TBD'}
                | Categories: 
                    {#if event.categories && event.categories.length > 0}
                        {event.categories.map(cat => cat.name).join(', ')}
                    {:else}
                        No categories
                    {/if}
                </small>
                </div>
                <button
                    type="button"
                    class="edit-pen {editingEvent === event.id ? 'active' : ''}"
                    on:click={() => {
                        if (editingEvent === event.id) {
                            editingEvent = null;
                            editingEventName = '';
                            editingEventDescription = null;
                            editingEventDateTime = null;
                            editingEventLocation = null;
                            editingEventCategoryIds = [];
                        } else {
                            editingEvent = event.id;
                            editingEventName = event.name;
                            editingEventDescription = event.description;
                            editingEventDateTime = new Date(event.dateTime).toISOString().slice(0, 16);
                            editingEventLocation = event.location;
                            editingEventCategoryIds = event.categories ? event.categories.map(cat => cat.id) : [];
                        }
                    }}
                >
                    ✏️
                </button>

            </div>
            
            {/each}

        </div>
           {#if eventId !=null}
                <button
                    type="button"
                    on:click={deleteEvents}
                    class="delete-button"
                    >
                    Delete Selected 
                </button>
            {/if}
            {#if editingEvent > 0}
            <h2>Alter event</h2>
                <form on:submit|preventDefault={alterEvents}>
                    <div>
                        <label for="eventName">Event Name:</label>
                        <input 
                            type="text"  
                            bind:value={editingEventName} 
                            required 
                            placeholder="Enter new event name"
                        >
                    </div>

                    <div>
                        <label for="eventDescription">Description:</label>
                        <textarea 
                            id="eventDescription" 
                            bind:value={editingEventDescription} 
                            placeholder="Enter event description"
                        ></textarea>
                    </div>

                    <div>
                        <label for="eventDateTime">Date & Time:</label>
                        <input 
                            type="datetime-local" 
                            id="eventDateTime" 
                            bind:value={editingEventDateTime} 
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
                                    bind:group={editingEventCategoryIds}
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
                            bind:value={editingEventLocation} 
                            required 
                            placeholder="Enter event location"
                        >
                    </div>
                    <button class="btn" type="submit" style="margin-top: 10px;">Update Event</button>
                </form>
            {/if}
        {:else}
            <p>No events found.</p>
        {/if}
    </div>

    <div style="margin-left: 350px; top: 15px;">
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

