
<script>
    import { onMount } from 'svelte';
    import {writable} from 'svelte/store';
    import {categories, events, setEvents, addEvent, deleteEvent, updateEvent } from '$lib/stores'
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
    let edetingEventDescription = null;
	
	
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
					description: edetingEventDescription,
					dateTime: new Date(eventDateTime),
					location: eventLocation,
					CategoryId: selectedCategoryIds.map(id => parseInt(id))
				})
            
            });
             if(response.ok){
                alert ("was here")

                const alteredEvent = await response.json();
        
                updateEvent(parseInt(alteredEvent.id), {name : alteredEvent.name})
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

<h1>Local Events</h1>
<div class="container">
   
    <div>
        <h2>Events</h2>
        {#if $events.length > 0}
         <div>
            {#each $events as event}
             <div class="checkbox-label">
                <input 
                    type="checkbox"
                    checked={eventId === event.id}
                     on:change={(e) => eventId = e.target.checked ? event.id : null}
                >
                <strong>{event.name}</strong> - {event.description}
                <br>
                <small>Date: {formatLocalDateTime(event.dateTime)} | Location: {event.location || 'TBD'}</small>
            <button
                    type="button"
                    class="edit-pen {editingEvent === event.id ? 'active' : ''}"
                    on:click={() => editingEvent = editingEvent === event.id ? null : event.id}  
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
                <form on:submit|preventDefault={alterEvents}>
                        <input 
                        type="text"  
                        bind:value={editingEventName} 
                        required 
                        placeholder="Enter New Name"
                        >
                        <button class="btn" type="submit" style="margin-top: 10px;">Update Category</button>
                    </form>
            {/if}
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

