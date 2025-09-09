<script>
	import { onMount } from 'svelte';

	let events = [];
	let categories = [];

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
				<strong>{event.title}</strong> - {event.description}
				<br>
				<small>Date: {event.startDate} | Location: {event.location || 'TBD'}</small>
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
