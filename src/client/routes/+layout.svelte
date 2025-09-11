<script>
	import favicon from '$lib/assets/favicon.svg';

	let { children } = $props();
</script>
<nav>
	<a href="/">home</a>
	<a href="/categories">category</a>
	<a href="/events">event</a>
	
</nav>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>




{@render children?.()}
