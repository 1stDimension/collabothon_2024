'use client';

import React, {useState} from 'react';
import {DndContext, DragOverEvent} from '@dnd-kit/core';

import {Droppable} from './dropable';
import {Draggable} from './draggable';

export default function DndArea() {
  const [isDropped, setIsDropped] = useState(false);
  const draggableMarkup = (
    <Draggable>Drag me</Draggable>
  );

  return (
    <DndContext onDragEnd={handleDragEnd}>
      {!isDropped ? draggableMarkup : null}
      <Droppable>
        {isDropped ? draggableMarkup : 'Drop here'}
      </Droppable>
    </DndContext>
  );

  function handleDragEnd(event: DragOverEvent) {
    if (event.over && event.over.id === 'droppable') {
      setIsDropped(true);
    }
  }
}