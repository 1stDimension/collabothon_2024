'use client';
import React, { useState } from 'react';
import { DndContext } from '@dnd-kit/core';
import { SortableContext, arrayMove, verticalListSortingStrategy } from '@dnd-kit/sortable';
import { Draggable } from './draggable';
import Approvals from './widgets/approvals';
import Exchange from './widgets/exchange';

export default function DndArea() {
  const [items, setItems] = useState(['approvals', 'exchange']);

  const handleDragEnd = (event: any) => {
    const { active, over } = event;
    if (over && active.id !== over.id) {
      setItems((items) => {
        const oldIndex = items.indexOf(active.id);
        const newIndex = items.indexOf(over.id);
        return arrayMove(items, oldIndex, newIndex);
      });
    }
  };

  return (
    <DndContext onDragEnd={handleDragEnd}>
      <SortableContext items={items} strategy={verticalListSortingStrategy}>
        <div className="grid grid-cols-3 gap-2">
          {items.map((id) => (
            <Draggable key={id} id={id}>
              {id === 'approvals' ? <Approvals /> : <Exchange />}
            </Draggable>
          ))}
        </div>
      </SortableContext>
    </DndContext>
  );
}
