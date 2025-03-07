// const BASE_URL = `${process.env.REACT_APP_BACK_END_SERVER_URL}/todos`;
const BASE_URL = `http://147.93.118.154:5036/todos`;

const getAllTodos = async () => {
	try {
		const res = await fetch(BASE_URL);
		return res.json();
	} catch (err) {
		console.log(err);
	}
};

const createTodo = async (data) => {
	try {
		const res = await fetch(BASE_URL, {
			method: 'POST',
			headers: {'Content-Type': 'application/json'},
			body: JSON.stringify(data),
		});
		return res.json();
	} catch (err) {
		console.log(err);
	}
};

const updateTodo = async (data, todoId) => {
	try {
		const res = await fetch(`${BASE_URL}/todo/${todoId}`, {
			method: 'PUT',
			headers: {'Content-Type': 'application/json'},
			body: JSON.stringify(data)
		});
		return res.json();
	} catch (err) {
		console.log(err);
	}
};

const deleteTodo = async (todoId) => {
	try {
		const deletedPet = await fetch(`${BASE_URL}/todo/${todoId}`, {
			method: 'DELETE',
			headers: {'Content-Type': 'application/json'},
		});
		return deletedPet;
	} catch (err) {
		console.log(err);
	}
}
export { getAllTodos, createTodo, updateTodo, deleteTodo};