import { useState, useEffect } from 'react';
import * as todoService from './services/todoService';

const App = () => {

	const initialTodo = {description: ''};
	const [todoArray, setTodoArray] = useState([]);
	const [todo, setTodo] = useState(initialTodo);

	const fetchTodos = async () => {
		
		const data = await todoService.getAllTodos();
		
		setTodoArray(data);
	};

	const handleDescriptionChange = (value) => {
		
		setTodo({
			...todo,
			description: value
		});
	};

	const handleAddTodo = async (todo) => {
		
		try {
			
			await todoService.createTodo(todo);
			
			// setTodoArray([newTodo, ...todoArray]);

			setTodo(initialTodo);
			
			fetchTodos();

		} catch (error) {console.log(error)}
	};

	const handleUpdateTodo = async (data, todoId) => {
		
		try {

			// const updatedTodo = await todoService.updateTodo(data, todoId);
			await todoService.updateTodo(data, todoId);

			// if (updatedTodo.error) throw new Error(updatedTodo.error);
	
			// const updatedTodoArray = todoArray.map((todo) =>
			// 	todo.id === updatedTodo.id ? updatedTodo: todo
			// );
			
			// setTodoArray(updatedTodoArray);

			fetchTodos();

		} catch (error) {console.log(error)}
	};
  
	const handleRemoveTodo = async (todoId) => {
		
		try {

			const deletedTodo = await todoService.deleteTodo(todoId);
	
			if (deletedTodo.error) throw new Error(deletedTodo.error);
			
			fetchTodos();
	
		} catch (error) {console.log(error)}
	}
  
	useEffect(() => {fetchTodos()}, []);
	
	return (
		<>
			<h1>Todos</h1>
			{todoArray.map((todo) => (
				<div key={todo.id}>
					<input
						type="checkbox"
						checked={todo.completed}
						onChange={(e) => handleUpdateTodo({...todo, completed: e.target.checked}, todo.id)}
					/>
					<p style={{textDecoration: todo.completed ? 'line-through' : 'none'}}>{todo.description}</p>
					<button onClick={() => handleUpdateTodo(todo)}>Edit</button>
					<button onClick={() => handleRemoveTodo(todo.id)}>Delete</button>
				</div>
			))}
			<input 
				type="text" 
				placeholder="Add a todo"
				value={todo.description}
				onChange={(e)=>handleDescriptionChange(e.target.value)} />
			<button onClick={() => handleAddTodo(todo)}>Add</button>
		</>
	)
}

export default App;