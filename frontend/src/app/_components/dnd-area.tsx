'use client';
import React, { useState } from 'react';
import { DndContext } from '@dnd-kit/core';
import { SortableContext, arrayMove, verticalListSortingStrategy } from '@dnd-kit/sortable';
import { Draggable } from './draggable';
import Approvals from './widgets/approvals';
import Exchange from './widgets/exchange';
import Accounts from './widgets/accounts';
import CreditCards from './widgets/credit-cards';
import Lawns from './widgets/lawns';

export default function DndArea() {
  const [items, setItems] = useState(['approvals', 'exchange', 'accounts', 'credit-cards', 'lawns']);

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
          {items.slice(0, 3).map((id, index) => (
            <Draggable key={`${id}-${index}`} id={id}>
              {id === 'approvals' ? <Approvals /> : id === 'exchange' ? <Exchange /> : id === 'accounts' ? <Accounts /> : <CreditCards />}
            </Draggable>
          ))}
        </div>
        <div className="grid grid-cols-2 gap-2 mt-2">
          {items.slice(3).map((id, index) => (
            <Draggable key={`${id}-${index + 3}`} id={id}>
              {id === 'lawns' ? <Lawns /> : id === 'approvals' ? <Approvals /> : id === 'exchange' ? <Exchange /> : id === 'accounts' ? <Accounts /> : <CreditCards />}
            </Draggable>
          ))}
        </div>
      </SortableContext>
    </DndContext>
  );
}
