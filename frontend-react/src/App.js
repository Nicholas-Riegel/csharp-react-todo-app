import { useState, useEffect } from 'react';
import * as todoService from './services/todoService';
import './App.css';

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
			
			setTodo(initialTodo);
			
			fetchTodos();

		} catch (error) {console.log(error)}
	};

	const handleUpdateTodo = async (data, todoId) => {
		
		try {

			await todoService.updateTodo(data, todoId);

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
		<div id='todos-wrapper'>
			<h1>Todos</h1>
			<div id='todos-container'>
				{todoArray && todoArray.map((todo) => (
					<div key={todo.id} id='todo'>
						<input
							type="checkbox"
							checked={todo.completed}
							onChange={(e) => handleUpdateTodo({...todo, completed: e.target.checked}, todo.id)}
						/>
						<span style={{textDecoration: todo.completed ? 'line-through' : 'none'}}>{todo.description}</span>
						{/* <button onClick={() => handleUpdateTodo(todo)}>Edit</button> */}
						<button onClick={() => handleRemoveTodo(todo.id)}>Delete</button>
					</div>
				))}
				<div id='add-todo-container'>
					<input 
						type="text" 
						placeholder="Add a todo"
						value={todo.description}
						onChange={(e)=>handleDescriptionChange(e.target.value)} />
					<button onClick={() => handleAddTodo(todo)}>Add</button>
				</div>
			</div>
		</div>
	)
}

export default App;