
<script>
    import { onMount } from 'svelte';
    import {writable} from 'svelte/store';
    import {categories, events, setEvents, addEvent, deleteEvent } from '$lib/stores'
    import '../app.css'; // from routes/ to app.css


	// Form data for new event
	let eventName = '';
	let eventDescription = '';
	let eventDateTime = '';
	let eventLocation = '';
    let eventId = null;
	let selectedCategoryIds = [];
	
	
	function formatLocalDateTime(dateTimeString) {
        if(!dateTimeString) return 'TBD';
        const date = new Date(dateTimeString);
        return date.toLocaleString(); 
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
             <div>
               
                <strong>{event.name}</strong> - {event.description}
                <br>
                <small>Date: {formatLocalDateTime(event.dateTime)} | Location: {event.location || 'TBD'}</small>

            </div>
            
            {/each}

         </div>
        {:else}
            <p>No events found.</p>
        {/if}
              
    </div>
</div>

