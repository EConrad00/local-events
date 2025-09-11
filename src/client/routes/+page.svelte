
<script>
    import { onMount } from 'svelte';
    import {writable} from 'svelte/store';
    import {categories, setCategories, events, setEvents} from '$lib/stores'
    import '../app.css'; // from routes/ to app.css


	// Form data for new event
	let eventName = '';
	let eventDescription = '';
	let eventDateTime = '';
	let eventLocation = '';
    let eventId = null;
	let selectedCategoryIds = [];
    let searchTerm = '';
	
	
	function formatLocalDateTime(dateTimeString) {
        if(!dateTimeString) return 'TBD';
        const date = new Date(dateTimeString);
        return date.toLocaleString(); 
    }

    $: filteredEvents = $events.filter(event => 
        event.name.toLowerCase().includes(searchTerm.toLowerCase())
    );

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
   
    <div>
        <h2>Events</h2>
        <div class="search-container">
            <input
            type="text"
            bind:value={searchTerm}
            placeholder="Search here..."
            class="search-input"
            >
            {#if searchTerm}
                <button
                    type="button"
                    class="clear-search"
                    on:click={() => searchTerm = ''}
                >
                    X
                </button>
            {/if}
        </div>
        {#if filteredEvents.length > 0}
         <div>
            {#each filteredEvents as event}
             <div>
               
                <strong>{event.name}</strong> - {event.description}
                <br>
                <small>Date: {formatLocalDateTime(event.dateTime)} | Location: {event.location || 'TBD'}</small>
                <br>
                <small> | Categories: 
                    {#if event.categories && event.categories.length > 0}
                        {event.categories.map(cat => cat.name).join(', ')}
                    {:else}
                        No categories
                    {/if}
                </small>

            </div>
            
            {/each}

         </div>
        {:else}
            <p>No events found.</p>
        {/if}
              
    </div>
</div>
